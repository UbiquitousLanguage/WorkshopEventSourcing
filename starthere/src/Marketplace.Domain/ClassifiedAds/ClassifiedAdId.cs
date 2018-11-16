using System;
using Marketplace.Framework;

namespace Marketplace.Domain.ClassifiedAds
{
    public class ClassifiedAdId : Value<ClassifiedAdId>
    {
        public static readonly ClassifiedAdId Default = new ClassifiedAdId(Guid.Empty);

        public readonly Guid Value;

        public ClassifiedAdId(Guid value) => Value = value;

        public static ClassifiedAdId Parse(Guid classifiedAdId)
        {
            if (classifiedAdId == Guid.Empty)
                throw new InvalidClassifiedAdId();

            return new ClassifiedAdId(classifiedAdId);
        }

        public static implicit operator Guid(ClassifiedAdId self) => self.Value;
        public static implicit operator ClassifiedAdId(Guid value) => Parse(value);
        public static implicit operator string(ClassifiedAdId self) => self.Value.ToString();
    }
}
