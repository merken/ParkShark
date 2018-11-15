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
            mapper.CreateMap<CreateDivisionDto, Division>((dto) => new Division(dto.Name, dto.OriginalName, dto.Director));
            mapper.CreateMap<Division, DivisionDto>((division) => new DivisionDto
            {
                Id = division.Id,
                Name = division.Name,
                OriginalName = division.OriginalName,
                Director = division.Director,
                ParentDivisionId = division.ParentDivisionId
            });
            mapper.CreateMap<CreateSubDivisionDto, Division>((dto) => new Division(dto.Name, dto.OriginalName, dto.Director, dto.ParentDivisionId));
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
