using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Marketplace.Framework;
using Marketplace.Infrastructure.RavenDB;
using Raven.Client.Documents.Session;
using static Marketplace.Domain.ClassifiedAds.Events;

namespace Marketplace.Modules.ClassifiedAds.Projections
{
    public class ClassifiedAdsByOwnerProjection : Projection
    {
        Func<IAsyncDocumentSession> GetSession { get; }

        public ClassifiedAdsByOwnerProjection(Func<IAsyncDocumentSession> getSession) => GetSession = getSession;

        public override Task Handle(object e)
        {
            switch (e)
            {
                case V1.ClassifiedAdRegistered x:
                    return GetSession.ThenSave<ClassifiedAdsByOwner>(ClassifiedAdsByOwner.Id(x.Owner), doc =>
                    {
                        doc.Ads.Add(new ClassifiedAd
                        {
                            Id = x.ClassifiedAdId,
                            RegisteredAt = x.RegisteredAt,
                            Status = ClassifiedAdStatus.Registered
                        });
                    });

                case V1.ClassifiedAdTitleChanged x:
                    return GetSession.ThenSave<ClassifiedAdsByOwner>(ClassifiedAdsByOwner.Id(x.Owner), doc =>
                    {
                        doc.Ads.Single(a => a.Id == x.ClassifiedAdId).Title = x.Title;
                    }, true);

                case V1.ClassifiedAdTextChanged x:
                    return GetSession.ThenSave<ClassifiedAdsByOwner>(ClassifiedAdsByOwner.Id(x.Owner), doc =>
                    {
                        doc.Ads.Single(a => a.Id == x.ClassifiedAdId).Text = x.Text;
                    }, true);

                case V1.ClassifiedAdPriceChanged x:
                    return GetSession.ThenSave<ClassifiedAdsByOwner>(ClassifiedAdsByOwner.Id(x.Owner), doc =>
                    {
                        doc.Ads.Single(a => a.Id == x.ClassifiedAdId).Price = x.Price;
                    }, true);

                case V1.ClassifiedAdPublished x:
                    return GetSession.ThenSave<ClassifiedAdsByOwner>(ClassifiedAdsByOwner.Id(x.Owner), doc =>
                    {
                        var ad = doc.Ads.Single(a => a.Id == x.ClassifiedAdId);
                        ad.PublishedAt = x.PublishedAt;
                        ad.Status = ClassifiedAdStatus.Published;
                    }, true);

                case V1.ClassifiedAdSold x:
                    return GetSession.ThenSave<ClassifiedAdsByOwner>(ClassifiedAdsByOwner.Id(x.Owner), doc =>
                    {
                        var ad = doc.Ads.Single(a => a.Id == x.ClassifiedAdId);
                        ad.SoldAt = x.SoldAt;
                        ad.Status = ClassifiedAdStatus.Sold;
                    });

                case V1.ClassifiedAdRemoved x:
                    return GetSession.ThenSave<ClassifiedAdsByOwner>(ClassifiedAdsByOwner.Id(x.Owner), doc =>
                    {
                        var ad = doc.Ads.Single(a => a.Id == x.ClassifiedAdId);
                        ad.RemovedAt = x.RemovedAt;
                        ad.Status = ClassifiedAdStatus.Removed;
                    });

                default:
                    return Task.CompletedTask;
            }
        }

        public override bool CanHandle(object e)
            => e is V1.ClassifiedAdRegistered
            || e is V1.ClassifiedAdTitleChanged
            || e is V1.ClassifiedAdTextChanged
            || e is V1.ClassifiedAdPriceChanged
            || e is V1.ClassifiedAdPublished
            || e is V1.ClassifiedAdSold
            || e is V1.ClassifiedAdRemoved;

        public class ClassifiedAdsByOwner
        {
            public static string Id(Guid id) => $"ClassifiedAdsByOwner/{id}";

            public List<ClassifiedAd> Ads { get; set; } = new List<ClassifiedAd>();
        }

        public class ClassifiedAd
        {
            public Guid Id { get; set; }
            public string Title { get; set; }
            public string Text { get; set; }
            public double Price { get; set; }

            public ClassifiedAdStatus Status { get; set; }
            public DateTimeOffset? SoldAt { get; set; }
            public DateTimeOffset RegisteredAt { get; set; }
            public DateTimeOffset? PublishedAt { get; set; }
            public DateTimeOffset? RemovedAt { get; set; }
        }

        public enum ClassifiedAdStatus { Registered, Published, Removed, Sold }
    }
}
