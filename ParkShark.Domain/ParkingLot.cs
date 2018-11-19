using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using ParkShark.Domain.Exceptions;

namespace ParkShark.Domain
{
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
