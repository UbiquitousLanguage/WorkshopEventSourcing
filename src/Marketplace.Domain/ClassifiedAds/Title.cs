using System;
using Marketplace.Framework;

namespace Marketplace.Domain.ClassifiedAds
{
    public class Title : Value<Title>
    {
        private string Value { get; }

        public static Title Parse(string value)
        {
            if (value.Length > 100)
                throw new ArgumentOutOfRangeException(nameof(value), "Too long title");

            return new Title(value);
        }

        internal Title(string value) => Value = value;

        public static implicit operator string(Title self) => self.Value;

        public static implicit operator Title(string value) => Parse(value);
    }
}
