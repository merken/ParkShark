using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ParkShark.Services;
using ParkShark.Web.Controllers;
using ParkShark.Web.DTO;

namespace ParkShark.Tests.UnitTests
{

    [TestClass]
    public class MemberTests : UnitTestBase
    {
        [TestMethod]
        public async Task MemberShouldBeCreated()
        {
            using (var context = await NewParkSharkInMemoryTestContext())
            {
                var memberService = new MemberService(context.ParkSharkDbContext);
                var membersController = new MembersController(context.Mapper, memberService);

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

                var newMember = GetResult<MemberDto>(await membersController.CreateNewMember(newMemberDto));
                var memberInDb = await context.ParkSharkDbContext.Members.FindAsync(newMember.Id);
                await context.ParkSharkDbContext.Entry(memberInDb).Reference(c => c.LicensePlate).LoadAsync();
                var contactInDb = await context.ParkSharkDbContext.Contacts.FindAsync(newMember.Contact.Id);
                await context.ParkSharkDbContext.Entry(contactInDb).Reference(c => c.Address).LoadAsync();
                var address = contactInDb.Address;

                Assert.AreNotEqual(default(int), newMember.Id);
                Assert.AreEqual(newMemberDto.ContactName, contactInDb.Name);
                Assert.AreEqual(newMemberDto.ContactEmail, contactInDb.Email);
                Assert.AreEqual(newMemberDto.LicensePlateCountry, memberInDb.LicensePlate.Country);
                Assert.AreEqual(newMemberDto.LicensePlateNumber, memberInDb.LicensePlate.Number);
                Assert.AreEqual(newMemberDto.ContactPostalCode, address.PostalCode);
                Assert.AreEqual(newMemberDto.ContactPostalName, address.PostalName);
            }
        }
    }
}
