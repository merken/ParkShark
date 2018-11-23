using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParkShark.Web.DTO
{
    public class CreateAllocationDto
    {
        public int MemberId { get; set; }
        public int ParkingLotId { get; set; }
        public string LicensePlateNumber { get; set; }
        public string LicensePlateCountry { get; set; }
    }
}
