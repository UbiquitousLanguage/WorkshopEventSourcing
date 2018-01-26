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

        public static ClassifiedAd Create(Guid id, Guid owner, Guid createdBy, DateTime createdAt)
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

        public void Rename(string title) =>
            Apply(new Events.V1.ClassifiedAdRenamed { Id = Id, Title = title });

        public void Publish(Guid publishedBy, DateTime publishedAt) =>
            Apply(new Events.V1.ClassifiedAdPublished
            {
                Id = Id,
                PublishedBy = publishedBy,
                PublishedAt = publishedAt
            });

        public void MarkAsSold(Guid markedBy, DateTime markedAt) =>
            Apply(new Events.V1.ClassifiedAdMarkedAsSold
            {
                Id = Id,
                MarkedAsSoldBy = markedBy,
                MarkedAsSoldAt = markedAt
            });
    }
}
