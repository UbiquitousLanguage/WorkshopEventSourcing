using Marketplace.Framework;

namespace Marketplace.Domain.ClassifiedAds
{
    public class Description : Value<Description>
    {
        public readonly string Value;

        public Description(string value)
        {
            Value = value;
        }

        public static implicit operator string(Description self) => self.Value;

        public static implicit operator Description(string value) => new Description(value);
    }
}
