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
