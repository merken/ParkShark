using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParkShark.Web.DTO
{
    public class CreateSubDivisionDto
    {
        public string Name { get; set; }
        public string OriginalName { get; set; }
        public string Director { get; set; }
        public int ParentDivisionId { get; set; }
    }
}
