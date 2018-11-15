using System;
using System.Collections.Generic;
using System.Text;

namespace ParkShark.Infrastructure.Exceptions
{
    public class PersistenceException : Exception
    {
        public PersistenceException(string message) : base($"Persistence did not succeed {message}")
        {

        }
    }
}
