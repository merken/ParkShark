using System;
using System.Collections.Generic;
using System.Text;
using ParkShark.Domain.Exceptions;

namespace ParkShark.Domain
{
    public static class BuildingTypes
    {
        public const int Underground = 1;
        public const int Aboveground = 2;
    }

    public class BuildingType
    {
        private BuildingType()
        {
            //This is for EF Core, so that it can be deserialized from db
        }

        public BuildingType(string name)
        {
            if (String.IsNullOrEmpty(name))
            {
                throw new ValidationException<BuildingType>("name is required");
            }

            this.Name = name;
        }

        public int Id { get; private set; }
        public string Name { get; set; }
    }
}
