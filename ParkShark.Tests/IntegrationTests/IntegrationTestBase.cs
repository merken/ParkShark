using System;
using System.Collections.Generic;
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
        [AssemblyInitialize]
        public static void AssemblyInit(TestContext testContext)
        {
            var options = new DbContextOptionsBuilder<ParkSharkDbContext>()
                .UseSqlServer(GetConfiguration().GetConnectionString("ParkSharkDb"))
                .Options;

            using (var context = new ParkSharkDbContext(options))
            {
                AddTestDataFromFile(context);
            }
        }

        private static IConfiguration GetConfiguration() => new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("testsettings.json")
            .Build();

        protected HttpClient SetupTestServerAndGetTestClient()
        {
            var builder = new WebHostBuilder()
                .UseEnvironment("Development")
                .UseConfiguration(GetConfiguration())
                .UseStartup<TestStartup>();

            var testServer = new TestServer(builder);
            return testServer.CreateClient();
        }

        private static void AddTestDataFromFile(ParkSharkDbContext context)
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

        protected static async Task<T> DeserializeAsAsync<T>(HttpContent content)
        {
            var responseContent = await content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(responseContent);
        }

        protected static HttpContent Serialize(object payload)
        {
            return new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
        }
    }
}
