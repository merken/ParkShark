using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParkShark.Web.DTO
{
    public class MemberShipLevelDto
    {
        public string Name { get; set; }
        public decimal MonthlyCost { get; set; }
        public decimal AllocationReduction { get; set; }
        public decimal MaximumDurationInMinutes { get; set; }
    }
}
