using System;
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
                throw new ValidationException<Division>("Name is required");
            }

            if (String.IsNullOrEmpty(director))
            {
                throw new ValidationException<Division>("Director is required");
            }

            this.Name = name;
            this.OriginalName = originalName;
            this.Director = director;
        }

        public int Id { get; private set; }
        public string Name { get; set; }
        public string OriginalName { get; set; }
        public string Director { get; set; }
    }
}
