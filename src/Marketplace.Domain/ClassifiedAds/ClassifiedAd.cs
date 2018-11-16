using System;
using Marketplace.Framework;

namespace Marketplace.Domain.ClassifiedAds
{
    public class ClassifiedAd : Aggregate
    {
        bool IsPublished { get; set; }
        bool WasSold { get; set; }
        bool WasRemoved { get; set; }

        Title Title { get; set; } = Title.Default;
        AdText Text { get; set; } = AdText.Default;
        Price Price { get; set; } = Price.Default;
        OwnerId Owner { get; set; } = OwnerId.Default;

        protected override void When(object e)
        {
            switch (e)
            {
                case Events.V1.ClassifiedAdRegistered x:
                    Id = new ClassifiedAdId(x.ClassifiedAdId);
                    Owner = new OwnerId(x.Owner);
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

                case Events.V1.ClassifiedAdPublished _:
                    IsPublished = true;
                    break;

                case Events.V1.ClassifiedAdSold _:
                    WasSold = true;
                    break;

                case Events.V1.ClassifiedAdRemoved _:
                    WasRemoved = true;
                    break;
            }
        }

        public void Register(ClassifiedAdId id, OwnerId owner, Func<DateTimeOffset> getUtcNow)
        {
            if (Version >= 0)
                throw new ClassifiedAdAlreadyRegistered();

            Apply(new Events.V1.ClassifiedAdRegistered
            {
                ClassifiedAdId = id,
                Owner = owner,
                RegisteredAt = getUtcNow()
            });
        }

        public void ChangeTitle(Title title, Func<DateTimeOffset> getUtcNow)
        {
            if (Version == -1)
                throw new ClassifiedAdNotFound();

            if (WasRemoved)
                throw new ClassifiedAdRemoved();

            if (Title != Title.Default && Title == title) return;

            Apply(new Events.V1.ClassifiedAdTitleChanged
            {
                ClassifiedAdId = Id,
                Owner = Owner,
                Title = title,
                ChangedAt = getUtcNow()
            });
        }

        public void ChangeText(AdText text, Func<DateTimeOffset> getUtcNow)
        {
            if (Version == -1)
                throw new ClassifiedAdNotFound();

            if (WasRemoved)
                throw new ClassifiedAdRemoved();

            if (Text != AdText.Default && Text == text) return;

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
                throw new ClassifiedAdNotFound();

            if (WasRemoved)
                throw new ClassifiedAdRemoved();

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
                throw new ClassifiedAdNotFound();

            if (WasRemoved)
                throw new ClassifiedAdRemoved();

            if (IsPublished) return;

            if (Title == Title.Default)
                throw new TitleRequired();

            Apply(new Events.V1.ClassifiedAdPublished
            {
                ClassifiedAdId = Id,
                Owner = Owner,
                Title = Title,
                Text = Text,
                PublishedAt = getUtcNow()
            });
        }

        public void MarkAsSold(Func<DateTimeOffset> getUtcNow)
        {
            if (Version == -1)
                throw new ClassifiedAdNotFound();

            if (WasRemoved)
                throw new ClassifiedAdRemoved();

            if (WasSold) return;

            if (!IsPublished)
                throw new ClassifiedAdUnpublished();

            Apply(new Events.V1.ClassifiedAdSold
            {
                Owner = Owner,
                ClassifiedAdId = Id,
                SoldAt = getUtcNow()
            });
        }

        public void Remove(Func<DateTimeOffset> getUtcNow)
        {
            if (Version == -1)
                throw new ClassifiedAdNotFound();

            if (WasRemoved) return;

            Apply(new Events.V1.ClassifiedAdRemoved
            {
                Owner = Owner,
                ClassifiedAdId = Id,
                RemovedAt = getUtcNow()
            });
        }
    }
}
