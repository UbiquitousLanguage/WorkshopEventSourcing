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
        private Func<IAsyncDocumentSession> GetSession { get; }

        public ClassifiedAdsByOwnerProjection(Func<IAsyncDocumentSession> getSession) => GetSession = getSession;

        public override Task Handle(object e)
        {
                switch (e)
                {
                    case V1.ClassifiedAdRegistered x:
                        return GetSession.ThenSave<ClassifiedAdsByOwner>(ClassifiedAdsByOwner.Id(x.Owner), doc =>
                        {
                            doc.OwnerId = x.Owner;
                            doc.Ads.Add(new ClassifiedAd
                            {
                                Id = x.ClassifiedAdId,
                                Title = x.Title,
                                Status = ClassifiedAdStatus.Registered
                            });
                        });

                    case V1.ClassifiedAdPriceChanged x:
                        return GetSession.ThenSave<ClassifiedAdsByOwner>(ClassifiedAdsByOwner.Id(x.Owner), doc =>
                        {
                            doc.Ads.Single(a => a.Id == x.ClassifiedAdId).Price = x.Price;
                        });

                    case V1.ClassifiedAdTitleChanged x:
                        return GetSession.ThenSave<ClassifiedAdsByOwner>(ClassifiedAdsByOwner.Id(x.Owner), doc =>
                        {
                            doc.Ads.Single(a => a.Id == x.ClassifiedAdId).Title = x.Title;
                        });

                    case V1.ClassifiedAdPublished x:
                        return GetSession.ThenSave<ClassifiedAdsByOwner>(ClassifiedAdsByOwner.Id(x.Owner), doc =>
                        {
                            doc.Ads.Single(a => a.Id == x.ClassifiedAdId).Status = ClassifiedAdStatus.Published;
                        });

                    case V1.ClassifiedAdSold x:
                        return GetSession.ThenSave<ClassifiedAdsByOwner>(ClassifiedAdsByOwner.Id(x.Owner), doc =>
                        {
                            doc.Ads.Single(a => a.Id == x.ClassifiedAdId).Status = ClassifiedAdStatus.Sold;
                        });

                    case V1.ClassifiedAdRemoved x:
                        return GetSession.ThenSave<ClassifiedAdsByOwner>(ClassifiedAdsByOwner.Id(x.Owner), doc =>
                        {
                            doc.Ads.Single(a => a.Id == x.ClassifiedAdId).Status = ClassifiedAdStatus.Removed;
                        });

                    default:
                        return Task.CompletedTask;
                }
        }

        public class ClassifiedAdsByOwner
        {
            public static string Id(Guid id) => $"ClassifiedAdsByOwner/{id}";

            public Guid OwnerId { get; set; }
            public List<ClassifiedAd> Ads { get; set; } = new List<ClassifiedAd>();
        }

        public class ClassifiedAd
        {
            public Guid Id { get; set; }
            public string Title { get; set; }
            public double Price { get; set; }
            public ClassifiedAdStatus Status { get; set; }
        }

        public enum ClassifiedAdStatus { Registered, Published, Removed, Sold }
    }
}
