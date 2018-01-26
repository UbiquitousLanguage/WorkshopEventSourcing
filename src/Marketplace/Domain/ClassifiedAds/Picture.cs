using Marketplace.Framework;

namespace Marketplace.Domain.ClassifiedAds
{
    public class Picture : Value<Picture>
    {
        public readonly string Description;
        public readonly string Url;

        public Picture(string url, string description)
        {
            Url = url;
            Description = description;
        }
    }
}
