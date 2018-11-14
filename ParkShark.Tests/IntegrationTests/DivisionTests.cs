using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ParkShark.Domain;

namespace ParkShark.Tests.IntegrationTests
{
    [TestClass]
    public class DivisionTests : IntegrationTestBase
    {
        [TestMethod]
        public async Task DivisionsShouldBeReturned()
        {
            using (var client = SetupTestServerAndGetTestClient())
            {
                var divisionsResponse = await client.GetAsync("api/divisions");
                var divisions = await DeserializeAsAsync<IEnumerable<Division>>(divisionsResponse.Content);

                var jobsDivision = divisions.FirstOrDefault(d => d.Director == "Steve Jobs");
                var flopsDivision = divisions.FirstOrDefault(d => d.Director == "Steve Flops");

                Assert.AreEqual("Apple", jobsDivision.Name);
                Assert.AreEqual("International Brol Machinekes", flopsDivision.Name);
            }
        }

        [TestMethod]
        public async Task DivisionShouldBeAdded()
        {
            using (var client = SetupTestServerAndGetTestClient())
            {
                var divisionToAdd = new Division
                {
                    Name = "New",
                    OriginalName = "Division",
                    Director = "John Doe"
                };

                var payload = Serialize(divisionToAdd);
                var divisionAddResponse = await client.PostAsync("api/divisions",payload);
                var division = await DeserializeAsAsync<Division>(divisionAddResponse.Content);


                Assert.AreEqual(divisionToAdd.Name, division.Name);
                Assert.AreEqual(divisionToAdd.OriginalName, division.OriginalName);
                Assert.AreEqual(divisionToAdd.Director, division.Director);
                Assert.AreNotEqual(default(int), division.Id);
            }
        }
    }
}
