using System;
using Marketplace.Framework;
using static System.String;

namespace Marketplace.Domain.ClassifiedAds
{
    public class Title : Value<Title>
    {
        public static readonly Title Default = new Title(Empty);

        private readonly string Value;
        
        internal Title(string value) => Value = value;

        public static Title Parse(string value)
        {
            if (value.Length > 100)
                throw new ArgumentOutOfRangeException(nameof(value), "Title is too long");

            return new Title(value);
        }
    
        public static implicit operator string(Title self) => self.Value;
        public static implicit operator Title(string value) => Parse(value);
    }
}
