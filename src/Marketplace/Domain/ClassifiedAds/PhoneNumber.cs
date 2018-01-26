namespace Marketplace.Domain.ClassifiedAds
{
    public class PhoneNumber
    {
        public readonly string Value;

        public PhoneNumber(string value)
        {
            Value = value;
        }

        public static implicit operator string(PhoneNumber self)
        {
            return self.Value;
        }

        public static implicit operator PhoneNumber(string value)
        {
            return new PhoneNumber(value);
        }
    }
}
