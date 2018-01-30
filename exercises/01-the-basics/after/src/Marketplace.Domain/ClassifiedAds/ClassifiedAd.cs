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
            switch (e)
            {
                case Events.V1.ClassifiedAdCreated x:
                    Id = x.Id;
                    _title = x.Title;
                    break;

                case Events.V1.ClassifiedAdRenamed x:
                    _title = x.Title;
                    break;

                case Events.V1.ClassifiedAdTextUpdated x:
                    _text = x.AdText;
                    break;

                case Events.V1.ClassifiedAdPriceChanged x:
                    _price = x.Price;
                    break;
            }
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
            if (Version == -1)
                throw new Exceptions.ClassifiedAdNotFoundException();       
            
            Apply(new Events.V1.ClassifiedAdRenamed
            {
                Id = Id,
                Title = title,
                RenamedAt = renamedAt,
                RenamedBy = renamedBy
            });
        }

        public void UpdateText(AdText text, DateTimeOffset updatedAt, UserId updatedBy) =>
            Apply(new Events.V1.ClassifiedAdTextUpdated
            {
                Id = Id,
                AdText = text,
                TextUpdatedAt = updatedAt,
                TextUpdatedBy = updatedBy
            });

        public void ChangePrice(Price price, DateTimeOffset changedAt, UserId changedBy) =>
            Apply(new Events.V1.ClassifiedAdPriceChanged
            {
                Id = Id,
                Price = price,
                PriceChangedAt = changedAt,
                PriceChangedBy = changedBy
            });
    }
}
