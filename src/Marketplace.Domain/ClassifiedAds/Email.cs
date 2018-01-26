using Marketplace.Framework;

namespace Marketplace.Domain.ClassifiedAds
{
    public class Email : Value<Email>
    {
        public readonly string Value;

        public Email(string value)
        {
            Value = value;
        }

        public static implicit operator string(Email self) => self.Value;

        public static implicit operator Email(string value) => new Email(value);
    }
}
