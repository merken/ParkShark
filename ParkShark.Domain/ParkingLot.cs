using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
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
                throw new ValidationException<Contact>("street is required");
            }

            if (String.IsNullOrEmpty(streetNumber))
            {
                throw new ValidationException<Contact>("streetNumber is required");
            }

            if (String.IsNullOrEmpty(postalCode))
            {
                throw new ValidationException<Contact>("postalCode is required");
            }

            if (String.IsNullOrEmpty(postalName))
            {
                throw new ValidationException<Contact>("postalName is required");
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

    public class Contact
    {
        private Contact()
        {
        }

        public Contact(string name, string mobilePhone, string phone, string email, Address address)
        {
            if (String.IsNullOrEmpty(name))
            {
                throw new ValidationException<Contact>("name is required");
            }

            if (!Validations.IsValidEmail(email))
            {
                throw new ValidationException<Contact>("email is not valid");
            }

            if (String.IsNullOrEmpty(mobilePhone) && String.IsNullOrEmpty(phone))
            {
                throw new ValidationException<Contact>("either provide a mobilePhone or a phone, both were not provided");
            }

            this.Name = name;
            this.MobilePhone = mobilePhone;
            this.Phone = phone;
            this.Email = email;
            this.Address = address;
        }

        public int Id { get; private set; }
        public string Name { get; set; }
        public string MobilePhone { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public Address Address { get; set; }
    }

    public class ParkingLot
    {
        private ParkingLot()
        {
        }

        public ParkingLot(string name, int divisionId, int contactId, int buildingTypeId, decimal pricePerHour)
        {
            if (String.IsNullOrEmpty(name))
            {
                throw new ValidationException<ParkingLot>("name is required");
            }

            if (default(int) == divisionId)
            {
                throw new ValidationException<ParkingLot>("divisionId is required");
            }

            if (default(int) == contactId)
            {
                throw new ValidationException<ParkingLot>("contactId is required");
            }

            if (default(int) == buildingTypeId)
            {
                throw new ValidationException<ParkingLot>("buildingTypeId is required");
            }

            if (default(decimal) == pricePerHour)
            {
                throw new ValidationException<ParkingLot>("pricePerHour is required");
            }

            this.Name = name;
            this.DivisionId = divisionId;
            this.ContactId = contactId;
            this.BuildingTypeId = buildingTypeId;
            this.PricePerHour = pricePerHour;
        }

        public ParkingLot(string name, int divisionId, Contact newContact, int buildingTypeId, decimal pricePerHour, decimal capacity)
        {
            if (String.IsNullOrEmpty(name))
            {
                throw new ValidationException<ParkingLot>("name is required");
            }

            if (default(int) == divisionId)
            {
                throw new ValidationException<ParkingLot>("divisionId is required");
            }

            if (newContact == null)
            {
                throw new ValidationException<ParkingLot>("newContact is required");
            }

            if (default(int) == buildingTypeId)
            {
                throw new ValidationException<ParkingLot>("buildingTypeId is required");
            }

            if (default(decimal) == pricePerHour)
            {
                throw new ValidationException<ParkingLot>("pricePerHour is required");
            }

            if (default(decimal) == capacity)
            {
                throw new ValidationException<ParkingLot>("capacity is required");
            }

            this.Name = name;
            this.DivisionId = divisionId;
            this.Contact = newContact;
            this.BuildingTypeId = buildingTypeId;
            this.PricePerHour = pricePerHour;
            this.Capacity = capacity;
        }

        public int Id { get; private set; }
        public string Name { get; set; }
        public int DivisionId { get; set; }
        public Division Division { get; private set; }
        public int ContactId { get; set; }
        public Contact Contact { get; set; }
        public int BuildingTypeId { get; set; }
        public BuildingType BuildingType { get; set; }
        public decimal PricePerHour { get; set; }
        public decimal Capacity { get; set; }
    }
}
