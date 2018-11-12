using System;
using System.Threading.Tasks;
using Marketplace.Domain.Shared.Services;
using Marketplace.Framework;

namespace Marketplace.Domain.ClassifiedAds
{
    public class ClassifiedAd : Aggregate
    {
        private bool IsPublished { get; set; }
        private bool WasSold { get; set; }
        private bool WasRemoved { get; set; }
        private Title Title { get; set; }
        private AdText Text { get; set; }
        private Price Price { get; set; }
        private Guid Owner { get; set; }
        
        protected override void When(object e)
        {
            switch (e)
            {
                case Events.V1.ClassifiedAdRegistered x:
                    Id = x.ClassifiedAdId;
                    Title = new Title(x.Title);
                    Owner = x.Owner;
                    break;

                case Events.V1.ClassifiedAdTitleChanged x:
                    Title = new Title(x.Title);
                    break;

                case Events.V1.ClassifiedAdTextChanged x:
                    Text = new AdText(x.Text);
                    break;

                case Events.V1.ClassifiedAdPriceChanged x:
                    Price = new Price(x.Price);
                    break;

                case Events.V1.ClassifiedAdPublished x:
                    IsPublished = true;
                    break;

                case Events.V1.ClassifiedAdSold x:
                    WasSold = true;
                    break;
            }
        }

        public static ClassifiedAd Register(ClassifiedAdId id, UserId owner, Func<DateTimeOffset> getUtcNow)
        {
            var ad = new ClassifiedAd();
            ad.Apply(new Events.V1.ClassifiedAdRegistered
            {
                ClassifiedAdId = id,
                Owner = owner,
                RegisteredAt = getUtcNow()
            });
            return ad;
        }

        public void ChangeTitle(Title title, Func<DateTimeOffset> getUtcNow)
        {
            if (Version == -1)
                throw new Exceptions.ClassifiedAdNotFoundException();

            if (Title != Title.Default && Title == title) return;
            
            Apply(new Events.V1.ClassifiedAdTitleChanged
            {
                ClassifiedAdId = Id,
                Owner = Owner,
                Title = title,
                ChangedAt = getUtcNow()
            });
        }

        public async Task ChangeText(AdText text, Func<DateTimeOffset> getUtcNow, CheckTextForProfanity checkTextForProfanity)
        {
            if (Version == -1)
                throw new Exceptions.ClassifiedAdNotFoundException(); 
            
            if (Text != AdText.Default && Text == text) return;
            
            var containsProfanity = await checkTextForProfanity(text);
            if (containsProfanity)
                throw new Exceptions.ProfanityFound();   
            
            Apply(new Events.V1.ClassifiedAdTextChanged
            {
                ClassifiedAdId = Id,
                Owner = Owner,
                Text = text,
                ChangedAt = getUtcNow()
            });
        }

        public void ChangePrice(Price price, Func<DateTimeOffset> getUtcNow)
        {
            if (Version == -1)
                throw new Exceptions.ClassifiedAdNotFoundException();   
            
            if (Price != Price.Default && Price == price) return;
            
            Apply(new Events.V1.ClassifiedAdPriceChanged
            {
                ClassifiedAdId = Id,
                Owner = Owner,
                Price = price,
                ChangedAt = getUtcNow()
            });
        }

        public void Publish(Func<DateTimeOffset> getUtcNow)
        {
            if (Version == -1)
                throw new Exceptions.ClassifiedAdNotFoundException();

            if (IsPublished) return;
            
            Apply(new Events.V1.ClassifiedAdPublished
            {
                ClassifiedAdId = Id,
                Title = Title,
                Text = Text,
                PublishedAt = getUtcNow()
            });
        }

        public void MarkAsSold(Func<DateTimeOffset> getUtcNow)
        {
            if (Version == -1)
                throw new Exceptions.ClassifiedAdNotFoundException();      
            
            if (WasSold) return;
            
            Apply(new Events.V1.ClassifiedAdSold
            {
                ClassifiedAdId = Id,
                SoldAt = getUtcNow()
            });
        }

        public void Remove(Func<DateTimeOffset> getUtcNow)
        {
            if (Version == -1)
                throw new Exceptions.ClassifiedAdNotFoundException();

            if (WasRemoved) return;
            
            Apply(new Events.V1.ClassifiedAdRemoved
            {
                ClassifiedAdId = Id,
                RemovedAt = getUtcNow()
            });
        }
    }
}
