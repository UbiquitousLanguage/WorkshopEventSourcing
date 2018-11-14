using System;

namespace Marketplace.Domain.ClassifiedAds
{  
    public class PriceNotAllowed : DomainException 
    {
        public PriceNotAllowed() 
            : base("Price not allowed.") { }
    }

    public class TitleTooLong : DomainException 
    {
        public TitleTooLong() 
            : base("Title too long.") { }
    }
    
    public class ClassifiedAdNotFound : DomainException 
    {
        public ClassifiedAdNotFound() 
            : base("Classified ad not found.") { }
    }
    
    public class ProfanityFound : DomainException 
    {
        public ProfanityFound() 
            : base("Profanity found.") { }
    }
    
    public abstract class DomainException : Exception
    {
        protected DomainException(string message) : base(message) { }
        
        public Guid Id { get; } = Guid.NewGuid();
    }
}
