﻿using System;
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

namespace ParkShark.Tests.IntegrationTests
{
    class TestData
    {
        public List<Division> Divisions { get; set; }
        public List<ParkingLot> ParkingLots { get; set; }
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
            List<ParkingLot> parkingLots = null;

            using (StreamReader reader = new StreamReader(@"testdata.json"))
            {
                string json = reader.ReadToEnd();
                var testData = JsonConvert.DeserializeObject<TestData>(json);
                divisions = testData.Divisions;
                parkingLots = testData.ParkingLots;
            }

            context.Divisions.RemoveRange(context.Divisions);
            context.Contacts.RemoveRange(context.Contacts);
            context.ParkingLots.RemoveRange(context.ParkingLots);
            context.SaveChanges();

            ReseedIdentity(context, "Divisions");
            ReseedIdentity(context, "Contacts");
            ReseedIdentity(context, "ParkingLots");

            if (divisions != null)
            {
                foreach (var division in divisions.OrderBy(d => d.Name))
                {
                    context.Divisions.Add(division);
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
