using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ParkShark.Domain;
using ParkShark.Web.Controllers;

namespace ParkShark.Tests.UnitTests
{
    [TestClass]
    public class DivisionTests : UnitTestBase
    {
        [TestMethod]
        public async Task DivisionsShouldBeReturned()
        {
            using (var context = NewInMemoryParkSharkDbContext())
            {
                await context.Divisions.AddAsync(new Division
                {
                    Name = "Apple",
                    Director = "Steve Jobs",
                    OriginalName = "Apple Computer"
                });

                await context.Divisions.AddAsync(new Division
                {
                    Name = "International Brol Machinekes",
                    Director = "Steve Flops",
                    OriginalName = "IBM"
                });

                await context.SaveChangesAsync();

                var controller = new DivisionsController(context);
                var divisions = await controller.GetDivisions();

                var jobsDivision = divisions.FirstOrDefault(d => d.Director == "Steve Jobs");
                var flopsDivision = divisions.FirstOrDefault(d => d.Director == "Steve Flops");
                Assert.AreEqual("Apple", jobsDivision.Name);
                Assert.AreEqual("International Brol Machinekes", flopsDivision.Name);
            }
        }
    }
}
