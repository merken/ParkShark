using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ParkShark.Domain;
using ParkShark.Infrastructure;
using ParkShark.Web.DTO;

namespace ParkShark.Tests.IntegrationTests
{
    [TestClass]
    public class DivisionIntegrationTests : IntegrationTestBase
    {
        protected override void ConfigureMappings(Mapper mapper)
        {
            base.ConfigureMappings(mapper);

            mapper.CreateMap<CreateDivisionDto, Division>((dto, m) => new Division(dto.Name, dto.OriginalName, dto.Director));
            mapper.CreateMap<Division, DivisionDto>((division, m) =>
            {
                var subDivisions = division.SubDivisions;

                var subDivisionDtos = new List<DivisionDto>();
                foreach (var subDivision in subDivisions)
                {
                    subDivisionDtos.Add(m.MapTo<DivisionDto, Division>(subDivision));
                }

                return new DivisionDto
                {
                    Id = division.Id,
                    Name = division.Name,
                    OriginalName = division.OriginalName,
                    Director = division.Director,
                    ParentDivisionId = division.ParentDivisionId,
                    SubDivisions = subDivisionDtos
                };
            });
            mapper.CreateMap<CreateSubDivisionDto, Division>((dto, m) => new Division(dto.Name, dto.OriginalName, dto.Director, dto.ParentDivisionId));
        }

        [TestMethod]
        public async Task DivisionsShouldBeReturned()
        {
            await RunWithinTransactionAndRollBack(async (client) =>
            {
                var divisionsResponse = await client.GetAsync("api/divisions");
                var divisions = await DeserializeAsAsync<IEnumerable<DivisionDto>>(divisionsResponse.Content);

                var jobsDivision = divisions.FirstOrDefault(d => d.Director == "Steve Jobs");
                var flopsDivision = divisions.FirstOrDefault(d => d.Director == "Steve Flops");

                Assert.AreEqual("Apple", jobsDivision.Name);
                Assert.AreEqual("International Brol Machinekes", flopsDivision.Name);
            });
        }

        [TestMethod]
        public async Task DivisionShouldBeAdded()
        {
            await RunWithinTransactionAndRollBack(async (client) =>
            {
                var divisionToAdd = new CreateDivisionDto
                {
                    Name = "New",
                    OriginalName = "Division",
                    Director = "John Doe"
                };

                var payload = Serialize(divisionToAdd);
                var divisionAddResponse = await client.PostAsync("api/divisions", payload);
                var division = await DeserializeAsAsync<DivisionDto>(divisionAddResponse.Content);


                Assert.AreEqual(divisionToAdd.Name, division.Name);
                Assert.AreEqual(divisionToAdd.OriginalName, division.OriginalName);
                Assert.AreEqual(divisionToAdd.Director, division.Director);
                Assert.AreNotEqual(default(int), division.Id);
            });
        }

        [TestMethod]
        public async Task DivisionShouldBeReturned()
        {
            await RunWithinTransactionAndRollBack(async (client) =>
            {
                var divisionsResponse = await client.GetAsync("api/divisions/1");
                var division = await DeserializeAsAsync<DivisionDto>(divisionsResponse.Content);

                Assert.AreEqual("International Brol Machinekes", division.Name);
                Assert.AreEqual("IBM", division.OriginalName);
                Assert.AreEqual("Steve Flops", division.Director);
            });
        }


        [TestMethod]
        public async Task DivisionsWithSubDivisionsShouldBeReturned()
        {
            await RunWithinTransactionAndRollBack(async (client) =>
            {
                var division1 = new CreateSubDivisionDto
                {
                    Name = "Apple1",
                    OriginalName = "Apple1",
                    Director = "Steve Jobs",
                    ParentDivisionId = 1
                };

                var division2 = new CreateSubDivisionDto
                {
                    Name = "Apple2",
                    OriginalName = "Apple2",
                    Director = "Steve Jobs",
                    ParentDivisionId = 1
                };

                var payload = Serialize(division1);
                await client.PostAsync("api/divisions/sub", payload);

                payload = Serialize(division2);
                await client.PostAsync("api/divisions/sub", payload);

                var divisionsResponse = await client.GetAsync("api/divisions/1");
                var division = await DeserializeAsAsync<DivisionDto>(divisionsResponse.Content);

                Assert.AreEqual(2, division.SubDivisions.Count);
            });
        }

        [TestMethod]
        public async Task SubDivisionShouldBeAdded()
        {
            await RunWithinTransactionAndRollBack(async (client) =>
            {
                var divisionToAdd = new CreateSubDivisionDto
                {
                    Name = "New",
                    OriginalName = "Division",
                    Director = "John Doe",
                    ParentDivisionId = 1
                };

                var payload = Serialize(divisionToAdd);
                var divisionAddResponse = await client.PostAsync("api/divisions/sub", payload);
                var division = await DeserializeAsAsync<DivisionDto>(divisionAddResponse.Content);

                Assert.AreEqual(divisionToAdd.Name, division.Name);
                Assert.AreEqual(divisionToAdd.OriginalName, division.OriginalName);
                Assert.AreEqual(divisionToAdd.Director, division.Director);
                Assert.AreEqual(1, division.ParentDivisionId);
                Assert.AreNotEqual(default(int), division.Id);
            });
        }
    }
}
