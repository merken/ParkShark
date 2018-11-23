using System;
using System.Collections.Generic;
using System.Text;

namespace ParkShark.Domain.Exceptions
{
    public class AllocationException : Exception
    {
        public AllocationException(string message) : base(message)
        {
        }
    }
}
