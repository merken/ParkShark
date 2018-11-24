using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParkShark.Web.DTO
{
    public class StopAllocationDto
    {
        public Guid AllocationId { get; set; }
        public int MemberId { get; set; }
    }
}
