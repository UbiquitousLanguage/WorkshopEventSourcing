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

        public static ClassifiedAd Create(ClassifiedAdId id, UserId owner, UserId createdBy)
        {
            var ad = new ClassifiedAd();
            ad.Apply(new Events.V1.ClassifiedAdCreated
            {
                Id = id,
                Owner = owner,
                CreatedBy = createdBy,
            });
            return ad;
        }

        public void Rename(Title title, UserId renamedBy)
        {
            if (Version == -1)
                throw new Exceptions.ClassifiedAdNotFoundException();       
            
            Apply(new Events.V1.ClassifiedAdRenamed
            {
                Id = Id,
                Title = title,
                RenamedBy = renamedBy
            });
        }

        public void UpdateText(AdText text, UserId updatedBy) =>
            Apply(new Events.V1.ClassifiedAdTextUpdated
            {
                Id = Id,
                AdText = text,
                TextUpdatedBy = updatedBy
            });

        public void ChangePrice(Price price, UserId changedBy) =>
            Apply(new Events.V1.ClassifiedAdPriceChanged
            {
                Id = Id,
                Price = price,
                PriceChangedBy = changedBy
            });

        public void Publish(UserId publishedBy) =>
            Apply(new Events.V1.ClassifiedAdPublished
            {
                Id = Id,
                PublishedBy = publishedBy,
                Title = _title,
                Text = _text
            });

        public void Activate(UserId activatedBy) =>
            Apply(new Events.V1.ClassifiedAdActivated
            {
                Id = Id,
                Title = _title,
                Price = _price,
                ActivatedBy = activatedBy
            });

        public void Reject(string reason, UserId rejectedBy) =>
            Apply(new Events.V1.ClassifiedAdRejected
            {
                Id = Id,
                Reason = reason,
                RejectedBy = rejectedBy,
            });

        public void Report(string reason, UserId reportedBy) =>
            Apply(new Events.V1.ClassifiedAdReportedByUser
            {
                Id = Id,
                Reason = reason,
                ReportedBy = reportedBy,
            });
        
        public void MarkAsSold(UserId markedBy) =>
            Apply(new Events.V1.ClassifiedAdMarkedAsSold
            {
                Id = Id,
                MarkedAsSoldBy = markedBy,
            });

        public void Deactivate(UserId deactivatedBy) =>
            Apply(new Events.V1.ClassifiedAdDeactivated
            {
                Id = Id,
                DeactivatedBy = deactivatedBy,
            });

        public void Remove(UserId removedBy) =>
            Apply(new Events.V1.ClassifiedAdRemoved
            {
                Id = Id,
                RemovedBy = removedBy,
            });
    }
}
