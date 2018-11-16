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
        private readonly Mapper mapper = null;

        public ParkingLotTests()
        {
            mapper = new Mapper();
            mapper.CreateMap<CreateNewParkingLotDto, ParkingLot>((dto, m) =>
            {
                var buildingTypeParsed = (BuildingType)Enum.Parse(typeof(BuildingType), dto.BuildingType);
                var address = new Address(dto.ContactStreet, dto.ContactStreetNumber, dto.ContactPostalCode,
                    dto.ContactPostalName);
                var contact = new Contact(dto.ContactName, dto.ContactMobilePhone, dto.ContactPhone, dto.ContactEmail,
                    address);

                return new ParkingLot(dto.Name, dto.DivisionId, contact, buildingTypeParsed, dto.PricePerHour, dto.Capacity);
            });

            mapper.CreateMap<Address, AddressDto>((address, m) => new AddressDto
            {
                Street = address.Street,
                StreetNumber = address.StreetNumber,
                PostalCode = address.PostalCode,
                PostalName = address.PostalName
            });

            mapper.CreateMap<Contact, ContactDto>((contact, m) =>
            {
                var addressDto = m.MapTo<AddressDto, Address>(contact.Address);
                return new ContactDto
                {
                    Id = contact.Id,
                    Email = contact.Email,
                    MobilePhone = contact.MobilePhone,
                    Name = contact.Name,
                    Phone = contact.Phone,
                    Address = addressDto
                };
            });

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

            mapper.CreateMap<ParkingLot, ParkingLotDto>((parkingLot, m) =>
            {
                var divisionDto = parkingLot.Division != null ? m.MapTo<DivisionDto, Division>(parkingLot.Division) : null;
                var contactDto = parkingLot.Contact != null ? m.MapTo<ContactDto, Contact>(parkingLot.Contact) : null;

                return new ParkingLotDto
                {
                    Id = parkingLot.Id,
                    Name = parkingLot.Name,
                    Division = divisionDto,
                    Contact = contactDto,
                    BuildingType = parkingLot.BuildingType.ToString(),
                    Capacity = parkingLot.Capacity,
                    PricePerHour = parkingLot.PricePerHour
                };
            });
        }

        [TestMethod]
        public async Task ParkingLotShouldBeCreated()
        {
            using (var context = NewInMemoryParkSharkDbContext())
            {
                var parkingLotService = new ParkingLotService(context);
                var parkingLotController = new ParkingLotsController(mapper, parkingLotService);

                var parkingLotDto = new CreateNewParkingLotDto
                {
                    Name = "PL2",
                    DivisionId = 2,
                    BuildingType = "Underground",
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
                var parkingLotInDb = await context.ParkingLots.FindAsync(newParkingLot.Id);
                var contactInDb = await context.Contacts.FindAsync(newParkingLot.Contact.Id);

                Assert.AreEqual(parkingLotInDb.Capacity, parkingLotDto.Capacity);
                Assert.AreEqual(parkingLotInDb.BuildingType.ToString(), parkingLotDto.BuildingType);
                Assert.AreEqual(contactInDb.Email, contactInDb.Email);
                Assert.AreEqual(contactInDb.Address.Street, contactInDb.Address.Street);
            }
        }

        [TestMethod]
        public async Task ParkingLotsShouldBeReturned()
        {
            using (var context = NewInMemoryParkSharkDbContext())
            {
                //Setup test data
                var division = new Division("Apple", "Apple Computer", "Steve Jobs");
                await context.Divisions.AddAsync(division);
                await context.ParkingLots.AddAsync(new ParkingLot(
                    "PL1",
                    division.Id,
                    new Contact("Maarten", "00554433", null, "merken.maarten@gmail.com", new Address("Streety", "Numbery", "Codey", "Namey")),
                    BuildingType.Underground,
                    15.55m,
                    500
                ));
                await context.ParkingLots.AddAsync(new ParkingLot(
                    "PL2",
                    division.Id,
                    new Contact("John", "005777433", null, "john.doe@gmail.com", new Address("Streety2", "Numbery3", "Codey4", "Namey5")),
                    BuildingType.Underground,
                    15.55m,
                    500
                ));
                await context.SaveChangesAsync();

                var parkingLotService = new ParkingLotService(context);

                var controller = new ParkingLotsController(mapper, parkingLotService);
                var parkingLots = GetResult<IEnumerable<ParkingLotDto>>((await controller.GetParkingLots()));

                Assert.AreEqual(2, parkingLots.Count());
                Assert.AreEqual(parkingLots.ElementAt(0).Name, "PL1");
                Assert.AreEqual(parkingLots.ElementAt(1).Name, "PL2");


            }
        }

    }
}
