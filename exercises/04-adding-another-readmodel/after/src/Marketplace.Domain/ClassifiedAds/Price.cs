using Marketplace.Framework;

namespace Marketplace.Domain.ClassifiedAds
{
    public class Price : Value<Price>
    {
        public double Value { get; }

        public Price(double value)
        {
            if (value <= 0d)
                throw new Exceptions.PriceNotAllowed();
            
            Value = value;
        }
        
        public static implicit operator double(Price self) => self.Value;

        public static implicit operator Price(double value) => new Price(value);
    }
}
