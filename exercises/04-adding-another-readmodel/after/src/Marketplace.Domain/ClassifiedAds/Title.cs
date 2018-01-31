using Marketplace.Framework;

namespace Marketplace.Domain.ClassifiedAds
{
    public class Title : Value<Title>
    {
        public string Value { get; }

        public Title(string value)
        {
            Value = value;
        }

        public static implicit operator string(Title self) => self.Value;

        public static implicit operator Title(string value) => new Title(value);
    }
}
