using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;
using ParkShark.Domain;
using ParkShark.Infrastructure;
using ParkShark.Web.DTO;

namespace ParkShark.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public readonly ILoggerFactory efLoggerFactory
            = new LoggerFactory(new[] { new ConsoleLoggerProvider((category, level) => category.Contains("Command") && level == LogLevel.Information, true) });

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureAdditionalServices(services);
        }

        protected virtual void ConfigureAdditionalServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            var connectionString = Configuration.GetConnectionString("ParkSharkDb");
            var mapper = new Mapper();
            ConfigureMappings(mapper);

            services.UseAllOfType<IParkSharkService>(new[] { typeof(ParkShark.Services.Assembly).Assembly }, ServiceLifetime.Transient);

            services.UseSqlServer(connectionString, efLoggerFactory);
            services.UseOneTransactionPerHttpCall();
            services.UseMapper(mapper);
        }

        protected virtual void ConfigureMappings(Mapper mapper)
        {
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

            mapper.CreateMap<CreateAllocationDto, Allocation>((dto, m) =>
            {
                var licensePlace = new LicensePlate(dto.LicensePlateNumber, dto.LicensePlateCountry);

                return new Allocation(dto.MemberId, dto.ParkingLotId, licensePlace, DateTime.Now);
            });

            mapper.CreateMap<Allocation, AllocationDto>((allocation, m) =>
            {
                var memberDto = allocation.Member != null ? m.MapTo<MemberDto, Member>(allocation.Member) : null;
                var parkingLotDto = allocation.ParkingLot != null ? m.MapTo<ParkingLotDto, ParkingLot>(allocation.ParkingLot) : null;
                var licensePlateDto = allocation.LicensePlate != null ? m.MapTo<LicensePlateDto, LicensePlate>(allocation.LicensePlate) : null;

                return new AllocationDto
                {
                    Id = allocation.Id,
                    Member = memberDto,
                    ParkingLot = parkingLotDto,
                    LicensePlate = licensePlateDto,
                    StartDateTime = allocation.StartDateTime,
                    EndDateTime = allocation.EndDateTime,
                    Status = allocation.Status.ToString()
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
