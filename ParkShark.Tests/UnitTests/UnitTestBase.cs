using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ParkShark.Data.Model;
using ParkShark.Domain;
using ParkShark.Infrastructure;
using ParkShark.Tests.Infrastructure;
using ParkShark.Web.DTO;

namespace ParkShark.Tests.UnitTests
{

    internal class UnitTestContext : IDisposable
    {
        public ParkSharkDbContext ParkSharkDbContext { get; set; }
        public Mapper Mapper { get; set; }

        public void Dispose()
        {
            ParkSharkDbContext?.Dispose();
            Mapper = null;
        }
    }

    public abstract class UnitTestBase
    {
        private static DbContextOptions CreateNewInMemoryDatabaseOptions()
        {
            return new DbContextOptionsBuilder<ParkSharkDbContext>()
                .UseInMemoryDatabase("ParkSharkDb" + Guid.NewGuid().ToString("N"))
                .Options;
        }

        internal async Task<UnitTestContext> NewParkSharkInMemoryTestContext()
        {
            var context = new ParkSharkDbContext(CreateNewInMemoryDatabaseOptions());

            var mapper = new Mapper();

            ConfigureMappings(mapper);

            context.ResetValueGenerators();

            await ProvideTestData(context);

            return new UnitTestContext
            {
                ParkSharkDbContext = context,
                Mapper = mapper
            };
        }

        protected static T GetResult<T>(ActionResult<T> actionResult)
            where T : class
        {
            var okResult = actionResult.Result as OkObjectResult;
            return okResult.Value as T;
        }

        protected async Task ProvideTestData(ParkSharkDbContext context)
        {
            await context.Divisions.AddAsync(new Division("First", "First", "Mr Test"));
            await context.SaveChangesAsync();
            await context.Divisions.AddAsync(new Division("Second", "Second", "Mr Test2"));
            await context.SaveChangesAsync();
            await context.Set<BuildingType>().AddAsync(new BuildingType(nameof(BuildingTypes.Underground)));
            await context.SaveChangesAsync();
            await context.Set<BuildingType>().AddAsync(new BuildingType(nameof(BuildingTypes.Aboveground)));
            await context.SaveChangesAsync();
        }

        protected static void ConfigureMappings(Mapper mapper)
        {
            mapper.CreateMap<CreateDivisionDto, Division>((dto, m) => new Division(dto.Name, dto.OriginalName, dto.Director));
            mapper.CreateMap<CreateSubDivisionDto, Division>((dto, m) => new Division(dto.Name, dto.OriginalName, dto.Director, dto.ParentDivisionId));
            mapper.CreateMap<CreateNewParkingLotDto, ParkingLot>((dto, m) =>
            {
                var address = new Address(dto.ContactStreet, dto.ContactStreetNumber, dto.ContactPostalCode,
                    dto.ContactPostalName);
                var contact = new Contact(dto.ContactName, dto.ContactMobilePhone, dto.ContactPhone, dto.ContactEmail,
                    address);

                return new ParkingLot(dto.Name, dto.DivisionId, contact, dto.BuildingTypeId, dto.PricePerHour, dto.Capacity);
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

            mapper.CreateMap<BuildingType, BuildingTypeDto>((buildingType, m) => new BuildingTypeDto
            {
                Id = buildingType.Id,
                Name = buildingType.Name
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
                var buildingTypeDto = parkingLot.BuildingType != null
                    ? m.MapTo<BuildingTypeDto, BuildingType>(parkingLot.BuildingType)
                    : null;
                return new ParkingLotDto
                {
                    Id = parkingLot.Id,
                    Name = parkingLot.Name,
                    Division = divisionDto,
                    Contact = contactDto,
                    BuildingType = buildingTypeDto,
                    Capacity = parkingLot.Capacity,
                    PricePerHour = parkingLot.PricePerHour
                };
            });

            mapper.CreateMap<CreateNewMemberDto, Member>((dto, m) =>
            {
                var address = new Address(dto.ContactStreet, dto.ContactStreetNumber, dto.ContactPostalCode,
                    dto.ContactPostalName);
                var contact = new Contact(dto.ContactName, dto.ContactMobilePhone, dto.ContactPhone, dto.ContactEmail,
                    address);
                var licensePlace = new LicensePlate(dto.LicensePlateNumber, dto.LicensePlateCountry);

                return new Member(contact, licensePlace, dto.RegistrationDate);
            });

            mapper.CreateMap<LicensePlate, LicensePlateDto>((licensePlate, m) => new LicensePlateDto
            {
                Country = licensePlate.Country,
                Number = licensePlate.Number
            });

            mapper.CreateMap<Member, MemberDto>((member, m) =>
            {
                var contactDto = member.Contact != null ? m.MapTo<ContactDto, Contact>(member.Contact) : null;
                var licensePlateDto = member.LicensePlate != null ? m.MapTo<LicensePlateDto, LicensePlate>(member.LicensePlate) : null;

                return new MemberDto
                {
                    Id = member.Id,
                    Contact = contactDto,
                    LicensePlate = licensePlateDto,
                    RegistrationDate = member.RegistrationDate
                };
            });
        }
    }
}
