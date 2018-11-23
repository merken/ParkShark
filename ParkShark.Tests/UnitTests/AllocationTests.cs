using System;
using System.Collections.Generic;
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

            var member = new Member(new Contact("Pl1", "124", "555", "test@gmail.com", new Address("street", "45", "555", "Test")), new LicensePlate("VXK014", "BE"), DateTime.Now, MemberShipLevel.Level.Bronze);

            await context.Members.AddAsync(member);
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
    }
}
