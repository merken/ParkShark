using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ParkShark.Web.DTO;

namespace ParkShark.Tests.IntegrationTests
{
    [TestClass]
    public class MemberIntegrationTests : IntegrationTestBase
    {
        [TestMethod]
        public async Task MemberShouldBeCreated()
        {
            await RunWithinTransactionAndRollBack(async (client) =>
            {
                var newMemberDto = new CreateNewMemberDto
                {
                    ContactName = "Maarten Merken",
                    ContactMobilePhone = "00486743685",
                    ContactEmail = "merken.maarten@gmail.com",
                    ContactStreet = "Aardeweg",
                    ContactStreetNumber = "39",
                    ContactPostalCode = "3582",
                    ContactPostalName = "Koersel",
                    LicensePlateNumber = "VXK155",
                    LicensePlateCountry = "BE",
                    RegistrationDate = DateTime.Now
                };

                var payload = Serialize(newMemberDto);
                var createMemberResponse = await client.PostAsync("api/members", payload);
                var memberDto = await DeserializeAsAsync<MemberDto>(createMemberResponse.Content);

                Assert.AreNotEqual(default(int), memberDto.Id);
                Assert.AreEqual(newMemberDto.ContactName, memberDto.Contact.Name);
                Assert.AreEqual(newMemberDto.ContactEmail, memberDto.Contact.Email);
                Assert.AreEqual(newMemberDto.LicensePlateCountry, memberDto.LicensePlate.Country);
                Assert.AreEqual(newMemberDto.LicensePlateNumber, memberDto.LicensePlate.Number);
                Assert.AreEqual(newMemberDto.ContactPostalCode, memberDto.Contact.Address.PostalCode);
                Assert.AreEqual(newMemberDto.ContactPostalName, memberDto.Contact.Address.PostalName);
            });
        }
    }
}
