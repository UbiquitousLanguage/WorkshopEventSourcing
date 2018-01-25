namespace Marketplace.Framework
{
    using System;

    public class WrongExpectedStreamVersionException : Exception
    {
        public WrongExpectedStreamVersionException(string message, Exception innerException = null) 
            : base(message, innerException) {}
    }
}