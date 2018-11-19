using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParkShark.Web.DTO
{
    public class CreateNewParkingLotDto
    {
        public string Name { get; set; }
        public int DivisionId { get; set; }
        public int BuildingTypeId { get; set; }
        public decimal Capacity { get; set; }
        public decimal PricePerHour { get; set; }
        public string ContactName { get; set; }
        public string ContactMobilePhone { get; set; }
        public string ContactPhone { get; set; }
        public string ContactEmail { get; set; }
        public string ContactStreet { get; set; }
        public string ContactStreetNumber { get; set; }
        public string ContactPostalCode { get; set; }
        public string ContactPostalName { get; set; }
    }
}
