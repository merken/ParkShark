using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ParkShark.Data.Model;
using ParkShark.Domain;
using ParkShark.Services;
using ParkShark.Web.Controllers;
using ParkShark.Web.DTO;

namespace ParkShark.Tests.UnitTests
{

    [TestClass]
    public class MemberTests : UnitTestBase
    {
        protected override async Task ProvideTestData(ParkSharkDbContext context)
        {
            await base.ProvideTestData(context);
            await context.Members.AddAsync(new Member(
                new Contact("Maarten Merken", "+3255511144", String.Empty, "merken.maarten@gmail.com",
                    new Address("Test", "15A", "1000", "Brussel")), new LicensePlate("VXK014", "BE"),
                DateTime.Now.AddYears(-15)));
            await context.SaveChangesAsync();
            await context.Members.AddAsync(new Member(
                new Contact("John Doe", "+3255511144", String.Empty, "john.doe@gmail.com",
                    new Address("Streety", "55", "3500", "Hasselt")), new LicensePlate("155542D4", "US"),
                DateTime.Now.AddYears(-1)));
            await context.SaveChangesAsync();
        }

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

        [TestMethod]
        public async Task MemberShouldBeReturned()
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

                var newMemberId = GetResult<MemberDto>(await membersController.CreateNewMember(newMemberDto)).Id;

                var member = GetResult<MemberDto>(await membersController.GetMember(newMemberId));

                Assert.AreNotEqual(default(int), member.Id);
                Assert.AreEqual(newMemberId, member.Id);
                Assert.AreEqual(newMemberDto.ContactName, member.Contact.Name);
                Assert.AreEqual(newMemberDto.ContactEmail, member.Contact.Email);
                Assert.AreEqual(newMemberDto.LicensePlateCountry, member.LicensePlate.Country);
                Assert.AreEqual(newMemberDto.LicensePlateNumber, member.LicensePlate.Number);
                Assert.AreEqual(newMemberDto.ContactPostalCode, member.Contact.Address.PostalCode);
                Assert.AreEqual(newMemberDto.ContactPostalName, member.Contact.Address.PostalName);

            }
        }

        [TestMethod]
        public async Task MembersShouldBeReturned()
        {
            using (var context = await NewParkSharkInMemoryTestContext())
            {
                var memberService = new MemberService(context.ParkSharkDbContext);
                var membersController = new MembersController(context.Mapper, memberService);

                var members = GetResult<IEnumerable<MemberDto>>(await membersController.GetAllMembers());
                Assert.AreEqual(2, members.Count());
                Assert.AreEqual("Maarten Merken", members.ElementAt(0).Contact.Name);
                Assert.AreEqual("VXK014", members.ElementAt(0).LicensePlate.Number);

                Assert.AreEqual("John Doe", members.ElementAt(1).Contact.Name);
                Assert.AreEqual("155542D4", members.ElementAt(1).LicensePlate.Number);
            }
        }
    }
}
