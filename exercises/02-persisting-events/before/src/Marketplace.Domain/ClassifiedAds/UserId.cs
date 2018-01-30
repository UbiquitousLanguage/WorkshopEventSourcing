using System;
using Marketplace.Framework;

namespace Marketplace.Domain.ClassifiedAds
{
    public class UserId : Value<UserId>
    {
        private readonly Guid _value; 
        
        public UserId(Guid value)
        {
            _value = value;
        }
        
        public static implicit operator Guid(UserId self) => self._value;

        public static implicit operator UserId(Guid value) => new UserId(value);
    }
}
