using System;
using Rating.Framework;

namespace Rating.Domain
{
    public class Rate : Value<Rate>
    {
        internal int Value { get; set; }

        public Rate(int value)
        {
            if (value <= 0 || value > 5)
                throw new ArgumentOutOfRangeException(nameof(value), "Rate must be between 1 and 5");

            Value = value;
        }

        internal Rate() { }

        public static implicit operator int(Rate self) => self.Value;
    }
}
