using System;
using System.Collections.Generic;
using System.Text;
using ParkShark.Domain.Exceptions;

namespace ParkShark.Domain
{
    public class Member
    {
        private Member() { }

        public Member(Contact contact, LicensePlate licensePlate, DateTime registrationDate, MemberShipLevel.Level memberShipLevel = Domain.MemberShipLevel.Level.Bronze)
        {
            if (contact == null)
            {
                throw new ValidationException<Member>("contact is required");
            }

            if (licensePlate == null)
            {
                throw new ValidationException<Member>("licensePlate is required");
            }

            if (default(DateTime) == registrationDate)
            {
                throw new ValidationException<Member>("registrationDate is required");
            }

            this.Contact = contact;
            this.LicensePlate = licensePlate;
            this.RegistrationDate = registrationDate;
            this.MemberShipLevel = memberShipLevel;
        }

        public int Id { get; private set; }
        public int ContactId { get; set; }
        public Contact Contact { get; set; }
        public LicensePlate LicensePlate { get; set; }
        public DateTime RegistrationDate { get; set; }
        public MemberShipLevel.Level MemberShipLevel { get; set; } = Domain.MemberShipLevel.Level.Bronze;
        public MemberShipLevel RelatedMemberShipLevel { get; set; }
    }
}
