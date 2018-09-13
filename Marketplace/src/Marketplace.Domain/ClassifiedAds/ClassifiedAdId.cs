using System;
using Marketplace.Framework;

namespace Marketplace.Domain.ClassifiedAds
{
    public class ClassifiedAdId : Value<ClassifiedAdId>
    {
        private readonly Guid _value; 
        
        public ClassifiedAdId(Guid value)
        {
            _value = value;
        }
        
        public static implicit operator Guid(ClassifiedAdId self) => self._value;

        public static implicit operator ClassifiedAdId(Guid value) => new ClassifiedAdId(value);
    }
}
