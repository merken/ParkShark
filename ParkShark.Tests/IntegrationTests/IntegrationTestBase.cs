using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
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
using ParkShark.Data.Model;
using ParkShark.Domain;
using ParkShark.Web;

namespace ParkShark.Tests.IntegrationTests
{
    class TestData
    {
        public List<Division> Divisions { get; set; }
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

        private static void PurgeDbAndAddTestDataFromFile(ParkSharkDbContext context)
        {
            List<Division> divisions = null;

            using (StreamReader reader = new StreamReader(@"testdata.json"))
            {
                string json = reader.ReadToEnd();
                var testData = JsonConvert.DeserializeObject<TestData>(json);
                divisions = testData.Divisions;
            }

            context.Divisions.RemoveRange(context.Divisions);

            if (divisions != null)
                context.Divisions.AddRange(divisions);

            context.SaveChanges();
        }

        protected static async Task RunWithinTransactionAndRollBack(Func<HttpClient, Task> codeToRun)
        {
            //Start a builder using the testsettings and the TestStartup class
            var builder = new WebHostBuilder()
                .UseEnvironment("Development")
                .UseConfiguration(GetConfiguration())
                .UseStartup<TestStartup>();

            //Create the TestServer
            var testServer = new TestServer(builder);
            //Get the singleton transaction out of the server
            var transaction = testServer.Host.Services.GetService<DbTransaction>();
            //Create a client to interact with the server
            var client = testServer.CreateClient();

            //Run your test code
            await codeToRun(client);

            //Roll back everything
            transaction.Rollback();
        }

        /// <summary>
        /// This handy little method allows you to deserialize a response content from the test server
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="content"></param>
        /// <returns></returns>
        protected static async Task<T> DeserializeAsAsync<T>(HttpContent content)
        {
            var responseContent = await content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(responseContent);
        }

        /// <summary>
        /// This handy little method allows you to serialize request content to send to the test server
        /// </summary>
        /// <param name="payload"></param>
        /// <returns></returns>
        protected static HttpContent Serialize(object payload)
        {
            return new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
        }
    }
}
