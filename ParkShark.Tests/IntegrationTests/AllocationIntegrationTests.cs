using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ParkShark.Web.DTO;

namespace ParkShark.Tests.IntegrationTests
{
    [TestClass]
    public class AllocationIntegrationTests : IntegrationTestBase
    {
        [TestMethod]
        public async Task AllocationShouldBeCreated()
        {
            await RunWithinTransactionAndRollBack(async (client) =>
            {
                var createAllocationDto = new CreateAllocationDto
                {
                    ParkingLotId = 1,
                    LicensePlateNumber = "VXK014",
                    MemberId = 2,
                    LicensePlateCountry = "BE"
                };

                var payload = Serialize(createAllocationDto);
                var allocationAddResponse = await client.PostAsync("api/allocations", payload);
                var allocation = await DeserializeAsAsync<AllocationDto>(allocationAddResponse.Content);


                Assert.AreEqual(createAllocationDto.ParkingLotId, allocation.ParkingLot.Id);
                Assert.AreEqual(createAllocationDto.MemberId, allocation.Member.Id);
                Assert.AreEqual(createAllocationDto.LicensePlateNumber, allocation.LicensePlate.Number);
                Assert.AreEqual(createAllocationDto.LicensePlateCountry, allocation.LicensePlate.Country);
            });
        }
    }
}
