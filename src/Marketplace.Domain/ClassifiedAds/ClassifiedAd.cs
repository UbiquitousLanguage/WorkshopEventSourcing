using System;
using System.Threading.Tasks;
using Marketplace.Domain.Shared.Services.ContentModeration;
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
        private Guid _owner;

        protected override void When(object e)
        {
            switch (e)
            {
                case Events.V1.ClassifiedAdCreated x:
                    Id = x.Id;
                    _title = new Title(x.Title);
                    _owner = x.Owner;
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

        public static ClassifiedAd Create(ClassifiedAdId id, UserId owner, Func<DateTimeOffset> getUtcNow)
        {
            var ad = new ClassifiedAd();
            ad.Apply(new Events.V1.ClassifiedAdCreated
            {
                Id = id,
                Owner = owner,
                CreatedBy = owner,
                CreatedAt = getUtcNow()
            });
            return ad;
        }

        public void Rename(Title title, UserId renamedBy, Func<DateTimeOffset> getUtcNow)
        {
            if (Version == -1)
                throw new Exceptions.ClassifiedAdNotFoundException();       
            
            Apply(new Events.V1.ClassifiedAdRenamed
            {
                Id = Id,
                Owner = _owner,
                Title = title,
                RenamedBy = renamedBy,
                RenamedAt = getUtcNow()
            });
        }

        public async Task UpdateText(AdText text, UserId updatedBy, Func<DateTimeOffset> getUtcNow, CheckTextForProfanity checkTextForProfanity)
        {
            var containsProfanity = await checkTextForProfanity(text);
            if (containsProfanity)
                throw new Exceptions.ProfanityFound();   
            
            Apply(new Events.V1.ClassifiedAdTextUpdated
            {
                Id = Id,
                Owner = _owner,
                AdText = text,
                TextUpdatedBy = updatedBy,
                TextUpdatedAt = getUtcNow()
            });
        }

        public void ChangePrice(Price price, UserId changedBy, Func<DateTimeOffset> getUtcNow) =>
            Apply(new Events.V1.ClassifiedAdPriceChanged
            {
                Id = Id,
                Owner = _owner,
                Price = price,
                PriceChangedBy = changedBy,
                PriceChangedAt = getUtcNow()
            });

        public void Publish(UserId publishedBy, Func<DateTimeOffset> getUtcNow) =>
            Apply(new Events.V1.ClassifiedAdPublished
            {
                Id = Id,
                Title = _title,
                Text = _text,  
                PublishedBy = publishedBy,
                PublishedAt = getUtcNow()
            });

        public void Activate(UserId activatedBy, Func<DateTimeOffset> getUtcNow)
        {
            if (_price == null)
                throw new Exceptions.ClassifiedAdActivationException("Price should be specified");
            
            if (string.IsNullOrEmpty(_title))
                throw new Exceptions.ClassifiedAdActivationException("Title cannot be empty");
                
            Apply(new Events.V1.ClassifiedAdActivated
            {
                Id = Id,
                Title = _title,
                Price = _price,
                ActivatedBy = activatedBy,
                ActivatedAt = getUtcNow()
            });
        }

        public void Reject(string reason, UserId rejectedBy, Func<DateTimeOffset> getUtcNow) =>
            Apply(new Events.V1.ClassifiedAdRejected
            {
                Id = Id,
                Reason = reason,
                RejectedBy = rejectedBy,
                RejectedAt = getUtcNow()
            });

        public void Report(string reason, UserId reportedBy, Func<DateTimeOffset> getUtcNow) =>
            Apply(new Events.V1.ClassifiedAdReportedByUser
            {
                Id = Id,
                Reason = reason,
                ReportedBy = reportedBy,
                ReportedAt = getUtcNow()
            });
        
        public void MarkAsSold(UserId markedBy, Func<DateTimeOffset> getUtcNow) =>
            Apply(new Events.V1.ClassifiedAdMarkedAsSold
            {
                Id = Id,
                MarkedAsSoldBy = markedBy,
                MarkedAsSoldAt = getUtcNow()
            });

        public void Deactivate(UserId deactivatedBy, Func<DateTimeOffset> getUtcNow) =>
            Apply(new Events.V1.ClassifiedAdDeactivated
            {
                Id = Id,
                DeactivatedBy = deactivatedBy,
                DeactivatedAt = getUtcNow()
            });

        public void Remove(UserId removedBy, Func<DateTimeOffset> getUtcNow) =>
            Apply(new Events.V1.ClassifiedAdRemoved
            {
                Id = Id,
                RemovedBy = removedBy,
                RemovedAt = getUtcNow()
            });
    }
}
