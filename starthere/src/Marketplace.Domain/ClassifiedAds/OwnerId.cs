using System;
using Marketplace.Framework;

namespace Marketplace.Domain.ClassifiedAds
{
    public class OwnerId : Value<OwnerId>
    {
        public static readonly OwnerId Default = new OwnerId(Guid.Empty);

        public readonly Guid Value;

        public OwnerId(Guid value) => Value = value;

        public static OwnerId Parse(Guid ownerId)
        {
            if (ownerId == Guid.Empty)
                throw new InvalidOwnerId();

            return new OwnerId(ownerId);
        }

        public static implicit operator Guid(OwnerId self) => self.Value;
        public static implicit operator OwnerId(Guid value) => Parse(value);
    }
}
