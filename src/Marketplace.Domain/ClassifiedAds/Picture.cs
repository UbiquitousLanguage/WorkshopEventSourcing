using System;
using Marketplace.Framework;

namespace Marketplace.Domain.ClassifiedAds
{
    public class Picture : Value<Picture>
    {
        public readonly string Url;
        public readonly string Description;

        internal Picture(string url, string description)
        {
            Url = url;
            Description = description;
        }

        public static Picture Parse(string url, string description)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(url));

            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(description));

            if(!Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute))
                throw  new ArgumentException("Invalid url", nameof(url));

            return new Picture(url, description);
        }
    }
}
