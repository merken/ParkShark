using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParkShark.Web.DTO
{
    public class CreateNewMemberDto
    {
        public string ContactName { get; set; }
        public string ContactMobilePhone { get; set; }
        public string ContactPhone { get; set; }
        public string ContactEmail { get; set; }
        public string ContactStreet { get; set; }
        public string ContactStreetNumber { get; set; }
        public string ContactPostalCode { get; set; }
        public string ContactPostalName { get; set; }
        public string LicensePlateNumber { get; set; }
        public string LicensePlateCountry { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string MemberShipLevel { get; set; }
    }
}
