using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    public class ParkingLotTests : UnitTestBase
    {
        [TestMethod]
        public async Task ParkingLotShouldBeCreated()
        {
            using (var context = await NewParkSharkInMemoryTestContext())
            {
                var parkingLotService = new ParkingLotService(context.ParkSharkDbContext);
                var parkingLotController = new ParkingLotsController(context.Mapper, parkingLotService);

                var parkingLotDto = new CreateNewParkingLotDto
                {
                    Name = "PL2",
                    DivisionId = 1,
                    BuildingTypeId = BuildingTypes.Underground,
                    Capacity = 500,
                    PricePerHour = 15.55m,
                    ContactName = "Maarten Merken",
                    ContactMobilePhone = "00486743685",
                    ContactEmail = "merken.maarten@gmail.com",
                    ContactStreet = "Aardeweg",
                    ContactStreetNumber = "39",
                    ContactPostalCode = "3582",
                    ContactPostalName = "Koersel"
                };

                var newParkingLot = GetResult<ParkingLotDto>(await parkingLotController.CreateNewParkingLot(parkingLotDto));
                var parkingLotInDb = await context.ParkSharkDbContext.ParkingLots.FindAsync(newParkingLot.Id);
                var contactInDb = await context.ParkSharkDbContext.Contacts.FindAsync(newParkingLot.Contact.Id);
                var buildingTypeInDb = await context.ParkSharkDbContext.Set<BuildingType>().FindAsync(newParkingLot.BuildingType.Id);

                Assert.AreEqual(parkingLotInDb.Capacity, parkingLotDto.Capacity);
                Assert.AreEqual(buildingTypeInDb.Name, nameof(BuildingTypes.Underground));
                Assert.AreEqual(contactInDb.Email, contactInDb.Email);
                Assert.AreEqual(contactInDb.Address.Street, contactInDb.Address.Street);
            }
        }

        [TestMethod]
        public async Task ParkingLotsShouldBeReturned()
        {
            using (var context = await NewParkSharkInMemoryTestContext())
            {
                //Setup test data
                var division = new Division("Apple", "Apple Computer", "Steve Jobs");
                await context.ParkSharkDbContext.Divisions.AddAsync(division);
                await context.ParkSharkDbContext.ParkingLots.AddAsync(new ParkingLot(
                    "PL1",
                    division.Id,
                    new Contact("Maarten", "00554433", null, "merken.maarten@gmail.com", new Address("Streety", "Numbery", "Codey", "Namey")),
                    BuildingTypes.Underground,
                    15.55m,
                    500
                ));
                await context.ParkSharkDbContext.ParkingLots.AddAsync(new ParkingLot(
                    "PL2",
                    division.Id,
                    new Contact("John", "005777433", null, "john.doe@gmail.com", new Address("Streety2", "Numbery3", "Codey4", "Namey5")),
                    BuildingTypes.Underground,
                    15.55m,
                    500
                ));
                await context.ParkSharkDbContext.SaveChangesAsync();

                var parkingLotService = new ParkingLotService(context.ParkSharkDbContext);

                var controller = new ParkingLotsController(context.Mapper, parkingLotService);
                var parkingLots = GetResult<IEnumerable<ParkingLotDto>>((await controller.GetParkingLots()));

                Assert.AreEqual(2, parkingLots.Count());
                Assert.AreEqual(parkingLots.ElementAt(0).Name, "PL1");
                Assert.AreEqual(parkingLots.ElementAt(1).Name, "PL2");
            }
        }

        [TestMethod]
        public async Task ParkingLotShouldBeReturned()
        {
            using (var context = await NewParkSharkInMemoryTestContext())
            {
                //Setup test data
                var division = new Division("Apple", "Apple Computer", "Steve Jobs");
                await context.ParkSharkDbContext.Divisions.AddAsync(division);
                var parkingLot1 = new ParkingLot(
                    "PL1",
                    division.Id,
                    new Contact("Maarten", "00554433", null, "merken.maarten@gmail.com",
                        new Address("Streety", "Numbery", "Codey", "Namey")),
                    BuildingTypes.Underground,
                    15.55m,
                    500
                );
                await context.ParkSharkDbContext.ParkingLots.AddAsync(parkingLot1);
                var parkingLot2 = new ParkingLot(
                    "PL2",
                    division.Id,
                    new Contact("John", "005777433", null, "john.doe@gmail.com",
                        new Address("Streety2", "Numbery3", "Codey4", "Namey5")),
                    BuildingTypes.Underground,
                    15.55m,
                    500
                );
                await context.ParkSharkDbContext.ParkingLots.AddAsync(parkingLot2);
                await context.ParkSharkDbContext.SaveChangesAsync();

                var parkingLotService = new ParkingLotService(context.ParkSharkDbContext);

                var controller = new ParkingLotsController(context.Mapper, parkingLotService);
                var parkingLot1Result = GetResult<ParkingLotDto>((await controller.GetParkingLot(parkingLot1.Id)));
                var parkingLot2Result = GetResult<ParkingLotDto>((await controller.GetParkingLot(parkingLot2.Id)));

                Assert.AreEqual(parkingLot1.Name, parkingLot1Result.Name);
                Assert.AreEqual(parkingLot1.Capacity, parkingLot1Result.Capacity);
                Assert.AreEqual(parkingLot1.BuildingType.Name, parkingLot1Result.BuildingType.Name);
                Assert.AreEqual(parkingLot1.Contact.Email, parkingLot1Result.Contact.Email);
                Assert.AreEqual(parkingLot1.Contact.Address.Street, parkingLot1Result.Contact.Address.Street);

                Assert.AreEqual(parkingLot2.Name, parkingLot2Result.Name);
                Assert.AreEqual(parkingLot2.Capacity, parkingLot2Result.Capacity);
                Assert.AreEqual(parkingLot2.BuildingType.Name, parkingLot2Result.BuildingType.Name);
                Assert.AreEqual(parkingLot2.Contact.Email, parkingLot2Result.Contact.Email);
                Assert.AreEqual(parkingLot2.Contact.Address.Street, parkingLot2Result.Contact.Address.Street);
            }
        }
    }
}
