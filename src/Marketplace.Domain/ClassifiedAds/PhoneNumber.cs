namespace Marketplace.Domain.ClassifiedAds
{
    public class PhoneNumber
    {
        public string Value { get; }

        public PhoneNumber(string value)
        {
            Value = value;
        }

        public static implicit operator string(PhoneNumber self) => self.Value;

        public static implicit operator PhoneNumber(string value) => new PhoneNumber(value);
    }
}
