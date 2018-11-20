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

        [TestMethod]
        public async Task MemberShouldBeReturned()
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
                var memberId = (await DeserializeAsAsync<MemberDto>(createMemberResponse.Content)).Id;

                var memberResponse = await client.GetAsync("api/members/"+ memberId);
                var member = await DeserializeAsAsync<MemberDto>(memberResponse.Content);

                Assert.AreNotEqual(default(int), member.Id);
                Assert.AreEqual(newMemberDto.ContactName, member.Contact.Name);
                Assert.AreEqual(newMemberDto.ContactEmail, member.Contact.Email);
                Assert.AreEqual(newMemberDto.LicensePlateCountry, member.LicensePlate.Country);
                Assert.AreEqual(newMemberDto.LicensePlateNumber, member.LicensePlate.Number);
                Assert.AreEqual(newMemberDto.ContactPostalCode, member.Contact.Address.PostalCode);
                Assert.AreEqual(newMemberDto.ContactPostalName, member.Contact.Address.PostalName);
            });
        }

        [TestMethod]
        public async Task MembersShouldBeReturned()
        {
            await RunWithinTransactionAndRollBack(async (client) =>
            {
                var membersResponse = await client.GetAsync("api/members");
                var members = await DeserializeAsAsync<IEnumerable<MemberDto>>(membersResponse.Content);

                Assert.AreEqual(2, members.Count());
                Assert.AreEqual("John Doe", members.ElementAt(0).Contact.Name);
                Assert.AreEqual("5511D54", members.ElementAt(0).LicensePlate.Number);

                Assert.AreEqual("Maarten Merken", members.ElementAt(1).Contact.Name);
                Assert.AreEqual("VXK014", members.ElementAt(1).LicensePlate.Number);
            });
        }
    }
}
