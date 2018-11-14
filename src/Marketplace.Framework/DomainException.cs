using System;

namespace Marketplace.Framework
{
    public abstract class DomainException : Exception
    {
        protected DomainException(string message) : base(message) { }

        public Guid Id { get; } = Guid.NewGuid();
    }
}
