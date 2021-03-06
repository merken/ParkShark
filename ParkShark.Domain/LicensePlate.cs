﻿using System;
using ParkShark.Domain.Exceptions;

namespace ParkShark.Domain
{
    public class LicensePlate
    {
        private LicensePlate() { }

        public LicensePlate(string number, string country)
        {
            if (String.IsNullOrEmpty(number))
            {
                throw new ValidationException<LicensePlate>("number is required");
            }

            if (String.IsNullOrEmpty(country))
            {
                throw new ValidationException<LicensePlate>("country is required");
            }

            this.Number = number;
            this.Country = country;
        }

        public string Number { get; set; }
        public string Country { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is LicensePlate other)
                return other.Number.Equals(this.Number) && other.Country.Equals(this.Country);

            return false;
        }
    }
}
