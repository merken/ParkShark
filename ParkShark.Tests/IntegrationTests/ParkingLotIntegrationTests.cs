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
        protected override void ConfigureMappings(Mapper mapper)
        {
            base.ConfigureMappings(mapper);

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
                    BuildingType = "Aboveground",
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
                Assert.AreEqual(parkingLotDto.BuildingType, parkingLot.BuildingType);
                Assert.AreEqual(parkingLotDto.ContactMobilePhone, parkingLot.Contact.MobilePhone);
                Assert.AreEqual(parkingLotDto.ContactStreet, parkingLot.Contact.Address.Street);
                Assert.AreNotEqual(default(int), parkingLot.Id);
            });
        }
    }
}
