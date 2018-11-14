using System;
using System.Collections.Generic;
using System.Text;

namespace ParkShark.Domain.Exceptions
{
    public class ValidationException<T> : Exception where T : class
    {
        public ValidationException(string validationMessage) : base($"Validation failed for entity {typeof(T).Name}: {validationMessage}")
        {

        }
    }
}
