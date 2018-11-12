using Marketplace.Framework;

namespace Marketplace.Domain.ClassifiedAds
{
    public class Price : Value<Price>
    {
        public readonly Price Default = new Price(0);

        private readonly double _value;

        internal Price(double value) => _value = value;

        public static Price Parse(double value)
        {
            if (value <= 0d)
                throw new Exceptions.PriceNotAllowed();

            return new Price(value);
        }

        public static implicit operator double(Price self) => self._value;
        public static implicit operator Price(double value) => Parse(value);
    }
}
