using Marketplace.Framework;
using static System.String;

namespace Marketplace.Domain.ClassifiedAds
{
    public class AdText : Value<AdText>
    {
        public static readonly AdText Default = new AdText(Empty);
        
        public readonly string Value;

        internal AdText(string value) => Value = value;
        
        public static implicit operator string(AdText self) => self.Value;
        public static implicit operator AdText(string value) => new AdText(value);
    }
}
