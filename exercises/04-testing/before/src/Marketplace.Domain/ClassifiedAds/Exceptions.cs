using System;

namespace Marketplace.Domain.ClassifiedAds
{
    public class Exceptions
    {
        public class PriceNotAllowed : Exception { }

        public class ClassifiedAdNotFoundException : Exception
        {
            public ClassifiedAdNotFoundException()
            {
                
            }
            
            public ClassifiedAdNotFoundException(Guid id) : base($"Classified ad with id '{id}' was not found")
            {
                
            }
        }
    }
}
