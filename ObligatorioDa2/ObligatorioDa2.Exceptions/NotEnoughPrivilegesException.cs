using System;

namespace ObligatorioDa2.Exceptions
{
    public class NotEnoughPrivilegesException : Exception
    {
        public NotEnoughPrivilegesException() : base()
        {

        }

        public NotEnoughPrivilegesException(string message) : base(message)
        {

        }

        public NotEnoughPrivilegesException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}
