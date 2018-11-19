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
    public class ParkingLotIntegrationTests : IntegrationTestBase
    {
        [TestMethod]
        public async Task ParkingLotsShouldBeReturned()
        {
            await RunWithinTransactionAndRollBack(async (client) =>
            {
                var parkingLotsResponse = await client.GetAsync("api/parkinglots");
                var parkingLots = await DeserializeAsAsync<IEnumerable<ParkingLotDto>>(parkingLotsResponse.Content);

                var first = parkingLots.FirstOrDefault(d => d.Name == "PL1");
                var second = parkingLots.FirstOrDefault(d => d.Name == "PL2");

                Assert.AreEqual("merken.maarten@gmail.com", first.Contact.Email);
                Assert.AreEqual("john.doe@gmail.com", second.Contact.Email);
            });
        }

        [TestMethod]
        public async Task ParkingLotShouldBeAdded()
        {
            await RunWithinTransactionAndRollBack(async (client) =>
            {
                var parkingLotDto = new CreateNewParkingLotDto
                {
                    Name = "PL3",
                    DivisionId = 1,
                    BuildingTypeId = BuildingTypes.Aboveground,
                    Capacity = 500,
                    PricePerHour = 15.55m,
                    ContactName = "Test Test",
                    ContactMobilePhone = "123456789",
                    ContactEmail = "abc.def@gmail.com",
                    ContactStreet = "Streety",
                    ContactStreetNumber = "55",
                    ContactPostalCode = "1000",
                    ContactPostalName = "Brussels"
                };

                var payload = Serialize(parkingLotDto);
                var parkingLotAddResponse = await client.PostAsync("api/parkinglots", payload);
                var parkingLot = await DeserializeAsAsync<ParkingLotDto>(parkingLotAddResponse.Content);

                Assert.AreEqual(parkingLotDto.Name, parkingLot.Name);
                Assert.AreEqual(parkingLotDto.Capacity, parkingLot.Capacity);
                Assert.AreEqual(parkingLotDto.BuildingTypeId, parkingLot.BuildingType.Id);
                Assert.AreEqual(parkingLotDto.ContactMobilePhone, parkingLot.Contact.MobilePhone);
                Assert.AreEqual(parkingLotDto.ContactStreet, parkingLot.Contact.Address.Street);
                Assert.AreNotEqual(default(int), parkingLot.Id);
            });
        }

        [TestMethod]
        public async Task ParkingLotShouldBeReturned()
        {
            await RunWithinTransactionAndRollBack(async (client) =>
            {
                var parkingLot1Response = await client.GetAsync("api/parkinglots/1");
                var parkingLot1 = await DeserializeAsAsync<ParkingLotDto>(parkingLot1Response.Content);
                var parkingLot2Response = await client.GetAsync("api/parkinglots/2");
                var parkingLot2 = await DeserializeAsAsync<ParkingLotDto>(parkingLot2Response.Content);

                Assert.AreEqual("PL1", parkingLot1.Name);
                Assert.AreEqual("merken.maarten@gmail.com", parkingLot1.Contact.Email);
                Assert.AreEqual("PL2", parkingLot2.Name);
                Assert.AreEqual("john.doe@gmail.com", parkingLot2.Contact.Email);
            });
        }

    }
}
