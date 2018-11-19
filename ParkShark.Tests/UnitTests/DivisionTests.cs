using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ParkShark.Domain;
using ParkShark.Infrastructure;
using ParkShark.Services;
using ParkShark.Web.Controllers;
using ParkShark.Web.DTO;

namespace ParkShark.Tests.UnitTests
{
    [TestClass]
    public class DivisionTests : UnitTestBase
    {
        [TestMethod]
        public async Task DivisionsShouldBeReturned()
        {
            using (var context = await NewParkSharkInMemoryTestContext())
            {
                //Setup test data
                await context.ParkSharkDbContext.Divisions.AddAsync(new Division("Apple", "Apple Computer", "Steve Jobs"));

                await context.ParkSharkDbContext.Divisions.AddAsync(new Division("International Brol Machinekes", "IBM", "Steve Flops"));

                await context.ParkSharkDbContext.SaveChangesAsync();

                var divisionService = new DivisionService(context.ParkSharkDbContext);

                var controller = new DivisionsController(context.Mapper, divisionService);
                var divisions = GetResult<IEnumerable<DivisionDto>>((await controller.GetDivisions()));

                var jobsDivision = divisions.FirstOrDefault(d => d.Director == "Steve Jobs");
                var flopsDivision = divisions.FirstOrDefault(d => d.Director == "Steve Flops");
                Assert.AreEqual("Apple", jobsDivision.Name);
                Assert.AreEqual("International Brol Machinekes", flopsDivision.Name);
            }
        }

        [TestMethod]
        public async Task DivisionsWithSubDivisionsShouldBeReturned()
        {
            using (var context = await NewParkSharkInMemoryTestContext())
            {
                //Setup test data
                var parentDivision = new Division("Parent", "Parent", "Steve Jobs");
                await context.ParkSharkDbContext.Divisions.AddAsync(parentDivision);
                await context.ParkSharkDbContext.Divisions.AddAsync(new Division("Child1", "Child1", "Steve Jobs", parentDivision.Id));
                await context.ParkSharkDbContext.Divisions.AddAsync(new Division("Child2", "Child2", "Steve Jobs", parentDivision.Id));

                await context.ParkSharkDbContext.SaveChangesAsync();

                var divisionService = new DivisionService(context.ParkSharkDbContext);

                var controller = new DivisionsController(context.Mapper, divisionService);
                var division = GetResult<DivisionDto>((await controller.GetDivision(parentDivision.Id)));

                Assert.AreEqual(2, division.SubDivisions.Count);
            }
        }

        [TestMethod]
        public async Task DivisionShouldBeCreated()
        {
            using (var context = await NewParkSharkInMemoryTestContext())
            {
                var divisionService = new DivisionService(context.ParkSharkDbContext);

                var controller = new DivisionsController(context.Mapper, divisionService);
                var division = GetResult<DivisionDto>(await controller.CreateDivision(new CreateDivisionDto
                {
                    Name = "Test",
                    Director = "Dir",
                    OriginalName = "Te"
                }));

                var divisionInDb = await context.ParkSharkDbContext.Divisions.FindAsync(division.Id);

                Assert.AreEqual("Test", division.Name);
                Assert.AreEqual("Dir", division.Director);
                Assert.AreEqual("Te", division.OriginalName);
                Assert.AreNotEqual(default(int), division.Id);
                Assert.AreEqual(division.Id, divisionInDb.Id);
            }
        }

        [TestMethod]
        public async Task DivisionShouldBeReturned()
        {
            using (var context = await NewParkSharkInMemoryTestContext())
            {
                //Setup test data
                await context.ParkSharkDbContext.Divisions.AddAsync(new Division("Apple", "Apple Computer", "Steve Jobs"));

                await context.ParkSharkDbContext.SaveChangesAsync();

                var steveJobsDivision = context.ParkSharkDbContext.Divisions.FirstOrDefault(d => d.Name == "Apple");

                var divisionService = new DivisionService(context.ParkSharkDbContext);

                var controller = new DivisionsController(context.Mapper, divisionService);
                var division = GetResult<DivisionDto>((await controller.GetDivision(steveJobsDivision.Id)));

                Assert.AreEqual("Apple", division.Name);
                Assert.AreEqual("Steve Jobs", division.Director);
                Assert.AreEqual("Apple Computer", division.OriginalName);
                Assert.AreEqual(steveJobsDivision.Id, division.Id);
            }
        }

        [TestMethod]
        public async Task SubDivisionShouldBeCreated()
        {
            using (var context = await NewParkSharkInMemoryTestContext())
            {
                var divisionService = new DivisionService(context.ParkSharkDbContext);

                var controller = new DivisionsController(context.Mapper, divisionService);
                var division = GetResult<DivisionDto>(await controller.CreateSubDivision(new CreateSubDivisionDto
                {
                    Name = "Test",
                    Director = "Dir",
                    OriginalName = "Te",
                    ParentDivisionId = 1
                }));

                var divisionInDb = await context.ParkSharkDbContext.Divisions.FindAsync(division.Id);

                Assert.AreEqual("Test", division.Name);
                Assert.AreEqual("Dir", division.Director);
                Assert.AreEqual("Te", division.OriginalName);
                Assert.AreNotEqual(default(int), division.Id);
                Assert.AreEqual(division.Id, divisionInDb.Id);
                Assert.AreEqual(1, divisionInDb.ParentDivisionId);
            }
        }
    }
}
