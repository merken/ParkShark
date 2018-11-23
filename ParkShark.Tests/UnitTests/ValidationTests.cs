using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ParkShark.Domain;
using ParkShark.Domain.Exceptions;

namespace ParkShark.Tests.UnitTests
{
    [TestClass]
    public class ValidationTests
    {
        [TestMethod]
        public void ContactValidationTests()
        {
            var validAddress = new Address("a", "b", "c", "d");
            Assert.ThrowsException<ValidationException<Contact>>(() =>
            {
                new Contact("", "b", "c", "d@abc.com", validAddress);
            });

            Assert.ThrowsException<ValidationException<Contact>>(() =>
            {
                new Contact("a", "", "", "d@abc.com", validAddress);
            });

            Assert.ThrowsException<ValidationException<Contact>>(() =>
            {
                new Contact("a", "b", "c", "d", validAddress);
            });

            Assert.ThrowsException<ValidationException<Contact>>(() =>
            {
                new Contact("a", "b", "c", "", validAddress);
            });

            //Succeeds
            new Contact("a", "b", "", "d@abc.com", validAddress);
            new Contact("a", "", "c", "d@abc.com", validAddress);
        }

        [TestMethod]
        public void MemberValidationTests()
        {
            var validAddress = new Address("a", "b", "c", "d");
            var validContact = new Contact("a", "b", "c", "d@abc.com", validAddress);
            var validLicensePlate = new LicensePlate("a", "b");

            Assert.ThrowsException<ValidationException<Member>>(() => { new Member(null, null, default(DateTime)); });
            Assert.ThrowsException<ValidationException<Member>>(() => { new Member(validContact, null, default(DateTime)); });
            Assert.ThrowsException<ValidationException<Member>>(() => { new Member(validContact, validLicensePlate, default(DateTime)); });

            //Succeeds
            new Member(validContact, validLicensePlate, DateTime.Now);
        }

        [TestMethod]
        public void AddressValidationTests()
        {
            Assert.ThrowsException<ValidationException<Address>>(() => { new Address("", "b", "c", "d"); });
            Assert.ThrowsException<ValidationException<Address>>(() => { new Address("a", "", "c", "d"); });
            Assert.ThrowsException<ValidationException<Address>>(() => { new Address("a", "b", "", "d"); });
            Assert.ThrowsException<ValidationException<Address>>(() => { new Address("a", "b", "c", ""); });

            //Succeeds
            new Address("a", "b", "c", "d");
        }

        [TestMethod]
        public void LicensePlateValidationTests()
        {
            Assert.ThrowsException<ValidationException<LicensePlate>>(() => { new LicensePlate("", "b"); });
            Assert.ThrowsException<ValidationException<LicensePlate>>(() => { new LicensePlate("a", ""); });

            //Succeeds
            new LicensePlate("a", "b");
        }

        [TestMethod]
        public void DivisionPlateValidationTests()
        {
            Assert.ThrowsException<ValidationException<Division>>(() => { new Division("a", "b", "c", 0); });
            Assert.ThrowsException<ValidationException<Division>>(() => { new Division("", "b", "c", 1); });
            Assert.ThrowsException<ValidationException<Division>>(() => { new Division("a", "b", "", 1); });

            //Succeeds
            new Division("a", "b", "c");
            new Division("a", "", "c");
            new Division("a", "b", "c", 1);
            new Division("a", "", "c", 1);
        }

        [TestMethod]
        public void MemberShipLevelValidationTests()
        {
            Assert.ThrowsException<ValidationException<MemberShipLevel>>(() => { new MemberShipLevel(MemberShipLevel.Level.Gold, 0, 0, 0); });

            //Succeeds
            new MemberShipLevel(MemberShipLevel.Level.Gold, 0, 0, 1);
        }

        [TestMethod]
        public void BuildingTypeValidationTests()
        {
            Assert.ThrowsException<ValidationException<BuildingType>>(() => { new BuildingType(""); });

            //Succeeds
            new BuildingType("a");
        }

        [TestMethod]
        public void AllocationValidationTests()
        {
            var validLicensePlate = new LicensePlate("a", "b");
            Assert.ThrowsException<ValidationException<Allocation>>(() => { new Allocation(0, 1, validLicensePlate, DateTime.Now); });
            Assert.ThrowsException<ValidationException<Allocation>>(() => { new Allocation(1, 0, validLicensePlate, DateTime.Now); });
            Assert.ThrowsException<ValidationException<Allocation>>(() => { new Allocation(1, 1, null, DateTime.Now); });
            Assert.ThrowsException<ValidationException<Allocation>>(() => { new Allocation(1, 1, validLicensePlate, default(DateTime)); });

            //Succeeds
            new Allocation(1, 1, validLicensePlate, DateTime.Now);
        }

        [TestMethod]
        public void ParkingLotValidationTests()
        {
            var validAddress = new Address("a", "b", "c", "d");
            var validContact = new Contact("a", "b", "c", "d@abc.com", validAddress);

            Assert.ThrowsException<ValidationException<ParkingLot>>(() => { new ParkingLot("", 1, validContact, 1, 1, 1); });
            Assert.ThrowsException<ValidationException<ParkingLot>>(() => { new ParkingLot("a", 0, validContact, 1, 1, 1); });
            Assert.ThrowsException<ValidationException<ParkingLot>>(() => { new ParkingLot("a", 1, null, 1, 1, 1); });
            Assert.ThrowsException<ValidationException<ParkingLot>>(() => { new ParkingLot("a", 1, validContact, 0, 1, 1); });
            Assert.ThrowsException<ValidationException<ParkingLot>>(() => { new ParkingLot("a", 1, validContact, 1, 0, 1); });
            Assert.ThrowsException<ValidationException<ParkingLot>>(() => { new ParkingLot("a", 1, validContact, 1, 1, 0); });

            //Succeeds
            new ParkingLot("a", 1, validContact, 1, 1, 1);
        }
    }
}
