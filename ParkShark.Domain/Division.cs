using System;
using System.Collections.Generic;
using System.Threading;
using ParkShark.Domain.Exceptions;

namespace ParkShark.Domain
{
    public class Division
    {
        private Division()
        {
            //This is for EF Core, so that it can be deserialized from db
        }

        public Division(string name, string originalName, string director)
        {

            if (String.IsNullOrEmpty(name))
            {
                throw new ValidationException<Division>("name is required");
            }

            if (String.IsNullOrEmpty(director))
            {
                throw new ValidationException<Division>("director is required");
            }

            this.Name = name;
            this.OriginalName = originalName;
            this.Director = director;
        }

        public Division(string name, string originalName, string director, int parentDivisionId): this(name, originalName, director)
        {
            if (default(int) == parentDivisionId)
            {
                throw new ValidationException<Division>("parentDivisionId is required");
            }

            this.ParentDivisionId = parentDivisionId;
        }

        public int Id { get; private set; }
        public string Name { get; set; }
        public string OriginalName { get; set; }
        public string Director { get; set; }
        public int? ParentDivisionId { get; set; }
        public Division ParentDivision { get; set; }
        public ICollection<Division> SubDivisions { get; } = new List<Division>();

    }
}
