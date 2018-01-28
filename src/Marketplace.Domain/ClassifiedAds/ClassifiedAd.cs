using System;
using System.Linq;
using Marketplace.Framework;

namespace Marketplace.Domain.ClassifiedAds
{
    public class ClassifiedAd : Aggregate
    {
        private bool _isPublished;
        private bool _sold;

        protected override void When(object e)
        {
            switch (e)
            {
                case Events.V1.ClassifiedAdCreated x:
                    Id = x.Id;
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

        public void Rename(Title title, DateTimeOffset renamedAt, UserId renamedBy) =>
            Apply(new Events.V1.ClassifiedAdRenamed
            {
                Id = Id,
                Title = title,
                RenamedAt = renamedAt,
                RenamedBy = renamedBy
            });

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

        public void Publish(UserId publishedBy, DateTimeOffset publishedAt) =>
            Apply(new Events.V1.ClassifiedAdPublished
            {
                Id = Id,
                PublishedBy = publishedBy,
                PublishedAt = publishedAt
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
