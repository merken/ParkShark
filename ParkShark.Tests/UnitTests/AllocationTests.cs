using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ParkShark.Data.Model;
using ParkShark.Domain;
using ParkShark.Domain.Exceptions;
using ParkShark.Services;

namespace ParkShark.Tests.UnitTests
{

    [TestClass]
    public class AllocationTests : UnitTestBase
    {
        protected override async Task ProvideTestData(ParkSharkDbContext context)
        {
            await base.ProvideTestData(context);

            var parkinglot = new ParkingLot("One spot", 1,
                new Contact("Pl1", "124", "555", "test@gmail.com", new Address("street", "45", "555", "Test")), 1,
                50m, 2);
            await context.ParkingLots.AddAsync(parkinglot);

            var member1 = new Member(new Contact("Pl1", "124", "555", "test@gmail.com", new Address("street", "45", "555", "Test")), new LicensePlate("VXK014", "BE"), DateTime.Now, MemberShipLevel.Level.Bronze);
            var member2 = new Member(new Contact("Pl1", "124", "555", "test@gmail.com", new Address("street", "45", "555", "Test")), new LicensePlate("VXK019", "BE"), DateTime.Now, MemberShipLevel.Level.Bronze);

            await context.Members.AddAsync(member1);
            await context.Members.AddAsync(member2);

            await context.SaveChangesAsync();
        }

        private async Task AllocationTestData(ParkSharkDbContext context)
        {
            await context.Allocations.AddAsync(new Allocation(1, 1, new LicensePlate("VXK014", "BE"), DateTime.Now.AddDays(-5)));
            await context.Allocations.AddAsync(new Allocation(1, 1, new LicensePlate("VXK014", "BE"), DateTime.Now.AddDays(-4)));
            await context.Allocations.AddAsync(new Allocation(2, 1, new LicensePlate("VXK019", "BE"), DateTime.Now.AddDays(-3)));
            var stopped1 = new Allocation(2, 1, new LicensePlate("VXK019", "BE"), DateTime.Now.AddDays(-2));
            stopped1.EndDateTime = DateTime.Now.AddDays(-2);
            var stopped2 = new Allocation(2, 1, new LicensePlate("VXK019", "BE"), DateTime.Now.AddDays(-1));
            stopped2.EndDateTime = DateTime.Now.AddDays(-1);
            await context.Allocations.AddAsync(stopped1);
            await context.Allocations.AddAsync(stopped2);
            await context.Allocations.AddAsync(new Allocation(1, 1, new LicensePlate("VXK014", "BE"), DateTime.Now));
            await context.SaveChangesAsync();
        }

        [TestMethod]
        public async Task AllocationShouldBeCreated()
        {
            using (var context = await NewParkSharkInMemoryTestContext())
            {
                var parkingLotService = new ParkingLotService(context.ParkSharkDbContext);
                var memberService = new MemberService(context.ParkSharkDbContext);
                var allocationService =
                    new AllocationService(context.ParkSharkDbContext, memberService, parkingLotService);

                var now = DateTime.Now;
                var allocation = await allocationService.CreateAllocation(new Allocation(1, 1, new LicensePlate("VXK014", "BE"), now));
                var allocationInDb = await context.ParkSharkDbContext.Allocations.FindAsync(allocation.Id);

                Assert.AreEqual(allocation.Id, allocationInDb.Id);
                Assert.AreEqual(1, allocationInDb.MemberId);
                Assert.AreEqual(1, allocationInDb.Member.Id);
                Assert.AreEqual(1, allocationInDb.ParkingLotId);
                Assert.AreEqual(1, allocationInDb.ParkingLot.Id);
                Assert.AreEqual(allocation.LicensePlate, allocationInDb.LicensePlate);
                Assert.AreEqual(now, allocationInDb.StartDateTime);
                Assert.AreEqual(null, allocationInDb.EndDateTime);
            }
        }

        [TestMethod]
        public async Task AllocationShouldBeCreated_Active()
        {
            using (var context = await NewParkSharkInMemoryTestContext())
            {
                var parkingLotService = new ParkingLotService(context.ParkSharkDbContext);
                var memberService = new MemberService(context.ParkSharkDbContext);
                var allocationService =
                    new AllocationService(context.ParkSharkDbContext, memberService, parkingLotService);

                var now = DateTime.Now;
                var allocation = await allocationService.CreateAllocation(new Allocation(1, 1, new LicensePlate("VXK014", "BE"), now));
                var allocationInDb = await context.ParkSharkDbContext.Allocations.FindAsync(allocation.Id);

                Assert.AreEqual(AllocationStatus.Active, allocation.Status);
                Assert.AreEqual(AllocationStatus.Active, allocationInDb.Status);
            }
        }

        [TestMethod]
        public async Task AllocationShouldBeStopped()
        {
            using (var context = await NewParkSharkInMemoryTestContext())
            {
                var parkingLotService = new ParkingLotService(context.ParkSharkDbContext);
                var memberService = new MemberService(context.ParkSharkDbContext);
                var allocationService =
                    new AllocationService(context.ParkSharkDbContext, memberService, parkingLotService);

                var now = DateTime.Now;
                var allocation = await allocationService.CreateAllocation(new Allocation(1, 1, new LicensePlate("VXK014", "BE"), now));

                var stoppedAllocation = await allocationService.StopAllocation(allocation.Id, 1);
                var allocationInDb = await context.ParkSharkDbContext.Allocations.FindAsync(allocation.Id);

                Assert.AreEqual(AllocationStatus.Passive, stoppedAllocation.Status);
                Assert.AreEqual(AllocationStatus.Passive, allocationInDb.Status);
            }
        }

        [TestMethod]
        public async Task AllocationShouldBeStopped_Fails_On_Member()
        {
            using (var context = await NewParkSharkInMemoryTestContext())
            {
                var parkingLotService = new ParkingLotService(context.ParkSharkDbContext);
                var memberService = new MemberService(context.ParkSharkDbContext);
                var allocationService =
                    new AllocationService(context.ParkSharkDbContext, memberService, parkingLotService);

                var now = DateTime.Now;
                var allocation = await allocationService.CreateAllocation(new Allocation(1, 1, new LicensePlate("VXK014", "BE"), now));

                await Assert.ThrowsExceptionAsync<AllocationException>(async () => await allocationService.StopAllocation(allocation.Id, 3));
            }
        }

        [TestMethod]
        public async Task AllocationShouldBeStopped_Fails_On_Passive()
        {
            using (var context = await NewParkSharkInMemoryTestContext())
            {
                var parkingLotService = new ParkingLotService(context.ParkSharkDbContext);
                var memberService = new MemberService(context.ParkSharkDbContext);
                var allocationService =
                    new AllocationService(context.ParkSharkDbContext, memberService, parkingLotService);

                var now = DateTime.Now;
                var allocation = await allocationService.CreateAllocation(new Allocation(1, 1, new LicensePlate("VXK014", "BE"), now));
                await allocationService.StopAllocation(allocation.Id, 1);

                await Assert.ThrowsExceptionAsync<AllocationException>(async () => await allocationService.StopAllocation(allocation.Id, 1));
            }
        }

        [TestMethod]
        public async Task AllocationShouldFail_Validations()
        {
            using (var context = await NewParkSharkInMemoryTestContext())
            {
                var parkingLotService = new ParkingLotService(context.ParkSharkDbContext);
                var memberService = new MemberService(context.ParkSharkDbContext);
                var allocationService =
                    new AllocationService(context.ParkSharkDbContext, memberService, parkingLotService);

                var now = DateTime.Now;
                var allocation1 = await allocationService.CreateAllocation(new Allocation(1, 1, new LicensePlate("VXK014", "BE"), now));
                var allocation2 = await allocationService.CreateAllocation(new Allocation(1, 1, new LicensePlate("VXK014", "BE"), now));

                await Assert.ThrowsExceptionAsync<ValidationException<Allocation>>(async () => await allocationService.CreateAllocation(new Allocation(1, 1, new LicensePlate("VXK914", "BE"), now)));
                await Assert.ThrowsExceptionAsync<AllocationException>(async () => await allocationService.CreateAllocation(new Allocation(1, 1, new LicensePlate("VXK014", "BE"), now)));
            }
        }

        [TestMethod]
        public async Task AllocationGetAllShouldReturnAll()
        {
            using (var context = await NewParkSharkInMemoryTestContext())
            {
                await AllocationTestData(context.ParkSharkDbContext);
                var parkingLotService = new ParkingLotService(context.ParkSharkDbContext);
                var memberService = new MemberService(context.ParkSharkDbContext);
                var allocationService =
                    new AllocationService(context.ParkSharkDbContext, memberService, parkingLotService);

                var allocations = await allocationService.GetAllocations();
                Assert.AreEqual(6, allocations.Count());
            }
        }

        [TestMethod]
        public async Task AllocationGetAllShouldReturnLimit2()
        {
            using (var context = await NewParkSharkInMemoryTestContext())
            {
                await AllocationTestData(context.ParkSharkDbContext);
                var parkingLotService = new ParkingLotService(context.ParkSharkDbContext);
                var memberService = new MemberService(context.ParkSharkDbContext);
                var allocationService =
                    new AllocationService(context.ParkSharkDbContext, memberService, parkingLotService);

                var allocations = await allocationService.GetAllocations(2);
                Assert.AreEqual(2, allocations.Count());
            }
        }

        [TestMethod]
        public async Task AllocationGetAllShouldReturnActive()
        {
            using (var context = await NewParkSharkInMemoryTestContext())
            {
                await AllocationTestData(context.ParkSharkDbContext);
                var parkingLotService = new ParkingLotService(context.ParkSharkDbContext);
                var memberService = new MemberService(context.ParkSharkDbContext);
                var allocationService =
                    new AllocationService(context.ParkSharkDbContext, memberService, parkingLotService);

                var allocations = await allocationService.GetAllocations(status: AllocationStatus.Active);
                Assert.AreEqual(4, allocations.Count());
            }
        }

        [TestMethod]
        public async Task AllocationGetAllShouldReturnPassive()
        {
            using (var context = await NewParkSharkInMemoryTestContext())
            {
                await AllocationTestData(context.ParkSharkDbContext);
                var parkingLotService = new ParkingLotService(context.ParkSharkDbContext);
                var memberService = new MemberService(context.ParkSharkDbContext);
                var allocationService =
                    new AllocationService(context.ParkSharkDbContext, memberService, parkingLotService);

                var allocations = await allocationService.GetAllocations(status: AllocationStatus.Passive);
                Assert.AreEqual(2, allocations.Count());
            }
        }
    }
}
