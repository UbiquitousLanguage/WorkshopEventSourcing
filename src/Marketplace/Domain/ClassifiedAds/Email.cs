namespace Marketplace.Domain.ClassifiedAds
{
    public class Email
    {
        public readonly string Value;

        public Email(string value)
        {
            Value = value;
        }

        public static implicit operator string(Email self)
        {
            return self.Value;
        }

        public static implicit operator Email(string value)
        {
            return new Email(value);
        }
    }
}
