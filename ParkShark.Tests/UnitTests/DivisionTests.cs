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
        private readonly Mapper mapper = null;

        public DivisionTests()
        {
            mapper = new Mapper();
            //Mappings for DTO and Entity
            mapper.CreateMap<CreateDivisionDto, Division>((dto, m) => new Division(dto.Name, dto.OriginalName, dto.Director));
            mapper.CreateMap<Division, DivisionDto>((division, m) => new DivisionDto
            {
                Id = division.Id,
                Name = division.Name,
                OriginalName = division.OriginalName,
                Director = division.Director,
                ParentDivisionId = division.ParentDivisionId
            });
            mapper.CreateMap<CreateSubDivisionDto, Division>((dto, m) => new Division(dto.Name, dto.OriginalName, dto.Director, dto.ParentDivisionId));
        }

        [TestMethod]
        public async Task DivisionsShouldBeReturned()
        {
            using (var context = NewInMemoryParkSharkDbContext())
            {
                //Setup test data
                await context.Divisions.AddAsync(new Division("Apple", "Apple Computer", "Steve Jobs"));

                await context.Divisions.AddAsync(new Division("International Brol Machinekes", "IBM", "Steve Flops"));

                await context.SaveChangesAsync();

                var divisionService = new DivisionService(context);

                var controller = new DivisionsController(mapper, divisionService);
                var divisions = GetResult<IEnumerable<DivisionDto>>((await controller.GetDivisions()));

                var jobsDivision = divisions.FirstOrDefault(d => d.Director == "Steve Jobs");
                var flopsDivision = divisions.FirstOrDefault(d => d.Director == "Steve Flops");
                Assert.AreEqual("Apple", jobsDivision.Name);
                Assert.AreEqual("International Brol Machinekes", flopsDivision.Name);
            }
        }

        [TestMethod]
        public async Task DivisionShouldBeCreated()
        {
            using (var context = NewInMemoryParkSharkDbContext())
            {
                var divisionService = new DivisionService(context);

                var controller = new DivisionsController(mapper, divisionService);
                var division = GetResult<DivisionDto>(await controller.CreateDivision(new CreateDivisionDto
                {
                    Name = "Test",
                    Director = "Dir",
                    OriginalName = "Te"
                }));

                var divisionInDb = await context.Divisions.FindAsync(division.Id);

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
            using (var context = NewInMemoryParkSharkDbContext())
            {
                //Setup test data
                await context.Divisions.AddAsync(new Division("Apple", "Apple Computer", "Steve Jobs"));

                await context.SaveChangesAsync();

                var steveJobsDivision = context.Divisions.FirstOrDefault(d => d.Name == "Apple");

                var divisionService = new DivisionService(context);

                var controller = new DivisionsController(mapper, divisionService);
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
            using (var context = NewInMemoryParkSharkDbContext())
            {
                var divisionService = new DivisionService(context);

                var controller = new DivisionsController(mapper, divisionService);
                var division = GetResult<DivisionDto>(await controller.CreateSubDivision(new CreateSubDivisionDto
                {
                    Name = "Test",
                    Director = "Dir",
                    OriginalName = "Te",
                    ParentDivisionId = 1
                }));

                var divisionInDb = await context.Divisions.FindAsync(division.Id);

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
