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

        [TestMethod]
        public async Task AllocationShouldBeCreated_Active()
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


                Assert.AreEqual("Active", allocation.Status);
            });
        }

        [TestMethod]
        public async Task AllocationShouldBeStopped()
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

                var stopAllocationDto = new StopAllocationDto
                {
                    AllocationId = allocation.Id,
                    MemberId = 2,
                };
                payload = Serialize(stopAllocationDto);
                var allocationStopResponse = await client.PutAsync("api/allocations", payload);
                allocation = await DeserializeAsAsync<AllocationDto>(allocationStopResponse.Content);

                Assert.AreEqual("Passive", allocation.Status);
            });
        }
    }
}
