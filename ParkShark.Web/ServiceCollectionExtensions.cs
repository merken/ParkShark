﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ParkShark.Data.Model;
using ParkShark.Infrastructure;
using ParkShark.Web.Filters;

namespace ParkShark.Web
{
    public static class ServiceCollectionExtensions
    {
        public static void UseAllOfType<T>(this IServiceCollection serviceCollection, Assembly[] assemblies, ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            var typesFromAssemblies = assemblies.SelectMany(a => a.DefinedTypes.Where(x => x.IsClass && x.GetInterfaces().Contains(typeof(T))));
            foreach (var type in typesFromAssemblies)
                serviceCollection.Add(new ServiceDescriptor(type.GetInterfaces()[0], type, lifetime));
        }

        public static void UseSqlServer(this IServiceCollection serviceCollection, string connectionString, ILoggerFactory loggerFactory, IsolationLevel level = IsolationLevel.ReadUncommitted)
        {
            //First, configure the SqlConnection and open it
            //This is done for every request/response
            serviceCollection.AddScoped<DbConnection>((serviceProvider) =>
            {
                var dbConnection = new SqlConnection(connectionString);
                dbConnection.Open();
                return dbConnection;
            });

            //Start a new transaction based on the SqlConnection
            //This is done for every request/response
            serviceCollection.AddScoped<DbTransaction>((serviceProvider) =>
            {
                var dbConnection = serviceProvider
                    .GetService<DbConnection>();

                return dbConnection.BeginTransaction(level);
            });

            //Create DbOptions for the DbContext, use the DbConnection
            //This is done for every request/response
            serviceCollection.AddScoped<DbContextOptions>((serviceProvider) =>
            {
                var dbConnection = serviceProvider.GetService<DbConnection>();
                return new DbContextOptionsBuilder<ParkSharkDbContext>()
                    .UseSqlServer(dbConnection)
                    .UseLoggerFactory(loggerFactory)
                    .Options;
            });

            //Finally, create the DbContext, using the transaction
            //This is done for every time a DbContext is requested (could be more than once per request/response)
            serviceCollection.AddScoped<ParkSharkDbContext>((serviceProvider) =>
            {
                var transaction = serviceProvider.GetService<DbTransaction>();
                var options = serviceProvider.GetService<DbContextOptions>();
                var context = new ParkSharkDbContext(options);
                context.Database.UseTransaction(transaction);
                return context;
            });
        }

        public static void UseOneTransactionPerHttpCall(this IServiceCollection serviceCollection, IsolationLevel level = IsolationLevel.ReadUncommitted)
        {
            //Manage the transaction at level of HTTP request/response
            //This is done for every request/response
            serviceCollection.AddScoped(typeof(TransactionFilter), typeof(TransactionFilter));

            serviceCollection
                .AddMvc(setup =>
                {
                    setup.Filters.AddService<TransactionFilter>(1);
                });
        }

        public static void UseMapper(this IServiceCollection serviceCollection, Mapper mapper)
        {
            serviceCollection.AddSingleton(mapper);
        }
    }
}
