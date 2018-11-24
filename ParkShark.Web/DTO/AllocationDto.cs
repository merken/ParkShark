using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParkShark.Web.DTO
{
    public class AllocationDto
    {
        public Guid Id { get; set; }
        public MemberDto Member { get; set; }
        public ParkingLotDto ParkingLot { get; set; }
        public LicensePlateDto LicensePlate { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
        public String Status { get; set; }
    }
}
