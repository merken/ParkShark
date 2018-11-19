using System;
using System.Collections.Generic;
using System.Text;
using ParkShark.Domain.Exceptions;

namespace ParkShark.Domain
{
    public class Member
    {
        private Member() { }

        public Member(Contact newContact, LicensePlate licensePlate, DateTime registrationDate)
        {
            if (newContact == null)
            {
                throw new ValidationException<Member>("newContact is required");
            }

            if (licensePlate == null)
            {
                throw new ValidationException<Member>("licensePlate is required");
            }

            if (default(DateTime) == registrationDate)
            {
                throw new ValidationException<Member>("registrationDate is required");
            }

            this.Contact = newContact;
            this.LicensePlate = licensePlate;
            this.RegistrationDate = registrationDate;
        }

        public int Id { get; private set; }
        public int ContactId { get; set; }
        public Contact Contact { get; set; }
        public LicensePlate LicensePlate { get; set; }
        public DateTime RegistrationDate { get; set; }
    }
}
