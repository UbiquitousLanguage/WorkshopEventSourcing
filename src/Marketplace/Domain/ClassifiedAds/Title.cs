namespace Marketplace.Domain.ClassifiedAds
{
    public class Title
    {
        public readonly string Value;

        public Title(string value)
        {
            Value = value;
        }

        public static implicit operator string(Title self)
        {
            return self.Value;
        }

        public static implicit operator Title(string value)
        {
            return new Title(value);
        }
    }
}
