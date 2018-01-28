using Marketplace.Framework;

namespace Marketplace.Domain.ClassifiedAds
{
    public class AdText : Value<AdText>
    {
        public readonly string Value;

        public AdText(string value)
        {
            Value = value;
        }

        public static implicit operator string(AdText self) => self.Value;

        public static implicit operator AdText(string value) => new AdText(value);
    }
}
