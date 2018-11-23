using System;
using ParkShark.Domain.Exceptions;

namespace ParkShark.Domain
{
    public class Address
    {
        public Address()
        {
        }

        public Address(string street, string streetNumber, string postalCode, string postalName)
        {
            if (String.IsNullOrEmpty(street))
            {
                throw new ValidationException<Address>("street is required");
            }

            if (String.IsNullOrEmpty(streetNumber))
            {
                throw new ValidationException<Address>("streetNumber is required");
            }

            if (String.IsNullOrEmpty(postalCode))
            {
                throw new ValidationException<Address>("postalCode is required");
            }

            if (String.IsNullOrEmpty(postalName))
            {
                throw new ValidationException<Address>("postalName is required");
            }

            this.Street = street;
            this.StreetNumber = streetNumber;
            this.PostalCode = postalCode;
            this.PostalName = postalName;
        }

        public string Street { get; set; }
        public string StreetNumber { get; set; }
        public string PostalCode { get; set; }
        public string PostalName { get; set; }
    }
}
