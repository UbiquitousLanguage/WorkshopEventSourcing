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

                case Events.V1.ClassifiedAdPublished x:
                    _isPublished = true;
                    break;

                case Events.V1.ClassifiedAdMarkedAsSold x:
                    _sold = true;
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

        public void Publish(DateTimeOffset publishedAt, UserId publishedBy) =>
            Apply(new Events.V1.ClassifiedAdPublished
            {
                Id = Id,
                PublishedBy = publishedBy,
                PublishedAt = publishedAt,
                Title = _title,
                Text = _text
            });

        public void Activate(DateTimeOffset activatedAt, UserId activatedBy) =>
            Apply(new Events.V1.ClassifiedAdActivated
            {
                Id = Id,
                Title = _title,
                Price = _price,
                ActivatedBy = activatedBy,
                ActivatedAt = activatedAt
            });

        public void Reject(string reason, UserId rejectedBy, DateTimeOffset rejectedAt) =>
            Apply(new Events.V1.ClassifiedAdRejected
            {
                Id = Id,
                Reason = reason,
                RejectedBy = rejectedBy,
                RejectedAt = rejectedAt
            });

        public void Report(string reason, UserId reportedBy, DateTimeOffset reportedAt) =>
            Apply(new Events.V1.ClassifiedAdReportedByUser
            {
                Id = Id,
                Reason = reason,
                ReportedBy = reportedBy,
                ReportedAt = reportedAt
            });
        
        public void MarkAsSold(UserId markedBy, DateTimeOffset markedAt) =>
            Apply(new Events.V1.ClassifiedAdMarkedAsSold
            {
                Id = Id,
                MarkedAsSoldBy = markedBy,
                MarkedAsSoldAt = markedAt
            });

        public void Deactivate(UserId deactivatedBy, DateTimeOffset deactivayedAt) =>
            Apply(new Events.V1.ClassifiedAdDeactivated
            {
                Id = Id,
                DeactivatedBy = deactivatedBy,
                DeactivatedAt = deactivayedAt
            });

        public void Remove(UserId removedBy, DateTimeOffset removedAt) =>
            Apply(new Events.V1.ClassifiedAdRemoved
            {
                Id = Id,
                RemovedBy = removedBy,
                RemovedAt = removedAt
            });
    }
}
