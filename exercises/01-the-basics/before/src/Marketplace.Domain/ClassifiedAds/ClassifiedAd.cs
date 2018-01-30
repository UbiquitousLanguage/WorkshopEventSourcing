using System;
using Marketplace.Framework;

namespace Marketplace.Domain.ClassifiedAds
{
    public class ClassifiedAd : Aggregate
    {
        private bool _isPublished;
        private bool _sold;
        private Title _title;
        private AdText _text;
        private Price _price;

        protected override void When(object e)
        {
        }

        public static ClassifiedAd Create(ClassifiedAdId id, UserId owner, UserId createdBy, DateTimeOffset createdAt)
        {
            var ad = new ClassifiedAd();
            ad.Apply(new Events.V1.ClassifiedAdCreated
            {
                Id = id,
                Owner = owner,
                CreatedBy = createdBy,
                CreatedAt = createdAt
            });
            return ad;
        }

        public void Rename(Title title, DateTimeOffset renamedAt, UserId renamedBy)
        {
        }

        public void UpdateText(AdText text, DateTimeOffset updatedAt, UserId updatedBy)
        {
        }

        public void ChangePrice(Price price, DateTimeOffset changedAt, UserId changedBy)
        {
        }
    }
}
