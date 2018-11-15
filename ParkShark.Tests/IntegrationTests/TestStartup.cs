using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ParkShark.Data.Model;
using ParkShark.Infrastructure;
using ParkShark.Web;
using ParkShark.Web.Controllers;

namespace ParkShark.Tests.IntegrationTests
{
    public class TestStartup : Startup
    {
        public TestStartup(IConfiguration configuration) : base(configuration)
        {
        }

        protected override void ConfigureAdditionalServices(IServiceCollection services)
        {
            var connectionString = Configuration.GetConnectionString("ParkSharkDb");

            services.UseAllOfType<IParkSharkService>(new[] { typeof(ParkShark.Services.Assembly).Assembly }, ServiceLifetime.Transient);
            
            //Register all ApiControllers from our Web project
            services.AddMvc().AddApplicationPart(typeof(Startup).Assembly);
            
            //Add a singleton DbConnection (one per test)
            services.AddSingleton<DbConnection>((serviceProvider) =>
            {
                var dbConnection = new SqlConnection(connectionString);
                dbConnection.Open();
                return dbConnection;
            });

            //Add a singleton Transaction (one per test)
            services.AddSingleton<DbTransaction>((serviceProvider) =>
            {
                var dbConnection = serviceProvider
                    .GetService<DbConnection>();

                return dbConnection.BeginTransaction(IsolationLevel.ReadCommitted);
            });

            //Create DbOptions for the DbContext, use the DbConnection
            //This is done for every request/response
            services.AddScoped<DbContextOptions>((serviceProvider) =>
            {
                var dbConnection = serviceProvider.GetService<DbConnection>();
                return new DbContextOptionsBuilder<ParkSharkDbContext>()
                    .UseSqlServer(dbConnection)
                    .Options;
            });

            //Finally, create the DbContext, using the one singleton transaction
            //This is done for every time a DbContext is requested (could be more than once per request/response)
            services.AddScoped<ParkSharkDbContext>((serviceProvider) =>
            {
                var options = serviceProvider.GetService<DbContextOptions>();
                var transaction = serviceProvider.GetService<DbTransaction>();
                var context = new ParkSharkDbContext(options);
                context.Database.UseTransaction(transaction);
                return context;
            });

            services.AddSingleton(new Mapper());
        }
    }
}
