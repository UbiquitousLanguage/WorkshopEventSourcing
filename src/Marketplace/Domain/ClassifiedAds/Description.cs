namespace Marketplace.Domain.ClassifiedAds
{
    public class Description
    {
        public readonly string Value;

        public Description(string value)
        {
            Value = value;
        }

        public static implicit operator string(Description self)
        {
            return self.Value;
        }

        public static implicit operator Description(string value)
        {
            return new Description(value);
        }
    }
}
