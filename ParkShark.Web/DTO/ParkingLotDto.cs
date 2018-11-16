using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParkShark.Web.DTO
{
    public class ParkingLotDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DivisionDto Division { get; set; }
        public ContactDto Contact { get; set; }
        public string BuildingType { get; set; }
        public decimal PricePerHour { get; set; }
        public decimal Capacity { get; set; }
    }
}
