using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParkShark.Web.DTO
{
    public class MemberDto
    {
        public int Id { get; set; }
        public ContactDto Contact { get; set; }
        public LicensePlateDto LicensePlate { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string MemberShipLevel { get; set; }
    }
}
