using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ParkShark.Data.Model;
using ParkShark.Domain;
using ParkShark.Infrastructure;
using ParkShark.Web;
using ParkShark.Web.DTO;

namespace ParkShark.Tests.IntegrationTests
{
    class TestData
    {
        public List<Division> Divisions { get; set; }
        public List<BuildingType> BuildingTypes { get; set; }
        public List<ParkingLot> ParkingLots { get; set; }
        public List<MemberShipLevel> MemberShipLevels { get; set; }
        public List<Member> Members { get; set; }
    }

    [TestClass]
    public abstract class IntegrationTestBase
    {
        private static IConfiguration GetConfiguration() => new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("testsettings.json")
            .Build();

        [AssemblyInitialize]
        public static void AssemblyInit(TestContext testContext)
        {
            var options = new DbContextOptionsBuilder<ParkSharkDbContext>()
                .UseSqlServer(GetConfiguration().GetConnectionString("ParkSharkDb"))
                .Options;

            using (var context = new ParkSharkDbContext(options))
            {
                PurgeDbAndAddTestDataFromFile(context);
            }
        }

        private static void ReseedIdentity(ParkSharkDbContext context, string table)
        {
            var parameter = new SqlParameter("@table", table);
            context.Database.ExecuteSqlCommand("DBCC CHECKIDENT(@table, RESEED, 0)", parameter);
        }

        private static void PurgeDbAndAddTestDataFromFile(ParkSharkDbContext context)
        {
            List<Division> divisions = null;
            List<BuildingType> buildingTypes = null;
            List<ParkingLot> parkingLots = null;
            List<MemberShipLevel> memberShipLevels = null;
            List<Member> members = null;

            using (StreamReader reader = new StreamReader(@"testdata.json"))
            {
                string json = reader.ReadToEnd();
                var testData = JsonConvert.DeserializeObject<TestData>(json);
                divisions = testData.Divisions;
                buildingTypes = testData.BuildingTypes;
                parkingLots = testData.ParkingLots;
                memberShipLevels = testData.MemberShipLevels;
                members = testData.Members;
            }

            context.Divisions.RemoveRange(context.Divisions);
            context.Set<BuildingType>().RemoveRange(context.Set<BuildingType>());
            context.Contacts.RemoveRange(context.Contacts);
            context.ParkingLots.RemoveRange(context.ParkingLots);
            context.Set<MemberShipLevel>().RemoveRange(context.Set<MemberShipLevel>());
            context.Members.RemoveRange(context.Members);
            context.SaveChanges();

            ReseedIdentity(context, "Divisions");
            ReseedIdentity(context, "BuildingTypes");
            ReseedIdentity(context, "Contacts");
            ReseedIdentity(context, "ParkingLots");
            ReseedIdentity(context, "Members");

            if (divisions != null)
            {
                foreach (var division in divisions.OrderBy(d => d.Name))
                {
                    context.Divisions.Add(division);
                    context.SaveChanges();
                }
            }

            if (buildingTypes != null)
            {
                foreach (var buildingType in buildingTypes)
                {
                    context.Set<BuildingType>().Add(buildingType);
                    context.SaveChanges();
                }
            }

            if (parkingLots != null)
            {
                foreach (var parkingLot in parkingLots.OrderBy(p => p.Name))
                {
                    context.ParkingLots.Add(parkingLot);
                    context.SaveChanges();
                }
            }

            if (memberShipLevels != null)
            {
                foreach (var memberShipLevel in memberShipLevels.OrderBy(p => (int)p.Name))
                {
                    context.Set<MemberShipLevel>().Add(memberShipLevel);
                    context.SaveChanges();
                }
            }

            if (members != null)
            {
                foreach (var member in members.OrderBy(p => p.Contact.Name))
                {
                    context.Members.Add(member);
                    context.SaveChanges();
                }
            }
        }

        protected async Task RunWithinTransactionAndRollBack(Func<HttpClient, Task> codeToRun)
        {
            //Start a builder using the testsettings and the TestStartup class
            var builder = new WebHostBuilder()
                .UseEnvironment("Development")
                .UseConfiguration(GetConfiguration())
                .UseStartup<TestStartup>();

            //Create the TestServer
            var testServer = new TestServer(builder);

            //Allow the test to configure the mappings
            var mapper = testServer.Host.Services.GetService<Mapper>();
            ConfigureMappings(mapper);

            //Get the singleton transaction out of the server
            var transaction = testServer.Host.Services.GetService<DbTransaction>();
            //Create a client to interact with the server
            var client = testServer.CreateClient();

            //Run your test code
            await codeToRun(client);

            //Roll back everything
            transaction.Rollback();
        }

        protected virtual void ConfigureMappings(Mapper mapper)
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

                if (!String.IsNullOrEmpty(dto.MemberShipLevel))
                {
                    var memberShipLevel = (MemberShipLevel.Level)Enum.Parse(typeof(MemberShipLevel.Level), dto.MemberShipLevel);
                    return new Member(contact, licensePlace, dto.RegistrationDate, memberShipLevel);
                }

                return new Member(contact, licensePlace, dto.RegistrationDate);
            });

            mapper.CreateMap<LicensePlate, LicensePlateDto>((licensePlate, m) => new LicensePlateDto
            {
                Country = licensePlate.Country,
                Number = licensePlate.Number
            });

            mapper.CreateMap<MemberShipLevel, MemberShipLevelDto>((memberShipLevel, m) => new MemberShipLevelDto
            {
                Name = memberShipLevel.Name.ToString(),
                MonthlyCost = memberShipLevel.MonthlyCost,
                AllocationReduction = memberShipLevel.AllocationReduction,
                MaximumDurationInMinutes = memberShipLevel.MaximumDurationInMinutes
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
                    RegistrationDate = member.RegistrationDate,
                    MemberShipLevel = member.MemberShipLevel.ToString()
                };
            });
        }

        /// <summary>
        /// This handy little method allows you to deserialize a response content from the test server
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="content"></param>
        /// <returns></returns>
        protected async Task<T> DeserializeAsAsync<T>(HttpContent content)
        {
            var responseContent = await content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(responseContent);
        }

        /// <summary>
        /// This handy little method allows you to serialize request content to send to the test server
        /// </summary>
        /// <param name="payload"></param>
        /// <returns></returns>
        protected HttpContent Serialize(object payload)
        {
            return new StringContent(JsonConvert.SerializeObject(payload, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }), Encoding.UTF8, "application/json");
        }
    }
}
