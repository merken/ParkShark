using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParkShark.Web.DTO
{
    public class DivisionDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string OriginalName { get; set; }
        public string Director { get; set; }
        public int? ParentDivisionId { get; set; }
        public List<DivisionDto> SubDivisions { get; set; } = new List<DivisionDto>();
    }
}
