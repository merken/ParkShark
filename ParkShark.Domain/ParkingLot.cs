using System;
using System.Collections.Generic;
using System.Text;

namespace ParkShark.Domain
{
    public enum BuildingType
    {
        None = 0,
        Underground,
        Aboveground
    }

    public class Address
    {
        public string Street { get; set; }
        public string StreetNumber { get; set; }
        public string PostalCode { get; set; }
        public string PostalName { get; set; }
    }

    public class Contact
    {
        public int Id { get; private set; }
        public string Name { get; set; }
        public string MobilePhone { get; set; }
        public string Phone { get; set; }
        //Owned Property
        public Address Address { get; set; }
    }

    public class ParkingLot
    {
        //Division required
        //Contact required
        //BuildingType cannot be none
        //PricePerHour required

        public int Id { get; private set; }
        public string Name { get; set; }
        public int DivisionId { get; set; }
        public Division Division { get; private set; }
        public int ContactId { get; set; }
        public Contact Contact { get; set; }
        public BuildingType BuildingType { get; set; }
        public decimal PricePerHour { get; set; }
    }
}
