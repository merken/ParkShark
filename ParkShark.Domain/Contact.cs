using System;
using ParkShark.Domain.Exceptions;

namespace ParkShark.Domain
{
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
}
