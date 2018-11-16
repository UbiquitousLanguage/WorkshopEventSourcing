using System;
using System.Threading.Tasks;
using Marketplace.Framework;
using Marketplace.Infrastructure.RavenDB;
using Raven.Client.Documents.Session;
using static Marketplace.Domain.ClassifiedAds.Events;

namespace Marketplace.Modules.ClassifiedAds.Projections
{
    public class AvailableClassifiedAdsProjection : Projection
    {
        Func<IAsyncDocumentSession> GetSession { get; }

        public AvailableClassifiedAdsProjection(Func<IAsyncDocumentSession> getSession) => GetSession = getSession;

        public override async Task Handle(object e)
        {
            switch (e)
            {
                case V1.ClassifiedAdRegistered x:
                    await GetSession.ThenSave<AvailableClassifiedAd>(
                        AvailableClassifiedAd.Id(x.ClassifiedAdId), doc =>
                        {
                            doc.Owner = x.Owner;
                            doc.ClassifiedAdId = x.ClassifiedAdId;
                        });
                    break;

                case V1.ClassifiedAdTitleChanged x:
                    await GetSession.ThenSave<AvailableClassifiedAd>(
                        AvailableClassifiedAd.Id(x.ClassifiedAdId), doc => doc.Title = x.Title);
                    break;

                case V1.ClassifiedAdTextChanged x:
                    await GetSession.ThenSave<AvailableClassifiedAd>(
                        AvailableClassifiedAd.Id(x.ClassifiedAdId), doc => doc.Text = x.Text);
                    break;

                case V1.ClassifiedAdPriceChanged x:
                    await GetSession.ThenSave<AvailableClassifiedAd>(
                        AvailableClassifiedAd.Id(x.ClassifiedAdId), doc => doc.Price = x.Price);
                    break;

                case V1.ClassifiedAdPublished x:
                    await GetSession.ThenSave<AvailableClassifiedAd>(
                        AvailableClassifiedAd.Id(x.ClassifiedAdId), doc => doc.PublishedAt = x.PublishedAt);
                    break;

                case V1.ClassifiedAdSold x:
                    await GetSession.ThenDelete(AvailableClassifiedAd.Id(x.ClassifiedAdId));
                    break;

                case V1.ClassifiedAdRemoved x:
                    await GetSession.ThenDelete(AvailableClassifiedAd.Id(x.ClassifiedAdId));
                    break;
            }
        }

        public override bool CanHandle(object e)
            => e is V1.ClassifiedAdRegistered
            || e is V1.ClassifiedAdPublished
            || e is V1.ClassifiedAdTitleChanged
            || e is V1.ClassifiedAdTextChanged
            || e is V1.ClassifiedAdPriceChanged
            || e is V1.ClassifiedAdRemoved
            || e is V1.ClassifiedAdSold;

        public class AvailableClassifiedAd
        {
            public static string Id(Guid id) => $"AvailableClassifiedAds/{id}";

            public Guid ClassifiedAdId { get; set; }
            public string Title { get; set; }
            public double Price { get; set; }
            public DateTimeOffset? PublishedAt { get; set; }
            public string Text { get; set; }
            public Guid Owner { get; set; }
        }
    }
}
