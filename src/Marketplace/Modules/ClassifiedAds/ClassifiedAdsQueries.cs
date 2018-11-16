using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Raven.Client.Documents;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Session;
using static Marketplace.Contracts.ClassifiedAds;

namespace Marketplace.Modules.ClassifiedAds
{
    public static class ClassifiedAdsQueries
    {
        public static async Task<V1.GetAvailableAds.Result> GetAvailableAds(
            this IAsyncDocumentSession session, V1.GetAvailableAds query, CancellationToken cancellationToken)
        {
            if (query.Page <= 0) query.Page = 1;
            if (query.PageSize <= 0) query.PageSize = 10;

            var result = await session.Query<Projections.AvailableClassifiedAdsProjection.AvailableClassifiedAd>()
                .Where(x => x.PublishedAt != null)
                .OrderByDescending(x => x.PublishedAt)
                .Statistics(out var statistics)
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(x => new V1.GetAvailableAds.Result.Item
                {
                    ClassifiedAdId = x.ClassifiedAdId,
                    Title = x.Title,
                    Text = x.Text,
                    Price = x.Price,
                    Owner = x.Owner,
                    PublishedAt = x.PublishedAt.Value
                })
                .ToListAsync(cancellationToken);

            return new V1.GetAvailableAds.Result
            {
                Page = query.Page,
                PageSize = query.PageSize,
                TotalItems = statistics.TotalResults - statistics.SkippedResults,
                TotalPages = (int) Math.Ceiling((double) statistics.TotalResults / query.PageSize),
                Items = result.ToArray()
            };
        }

        public static async Task<V1.GetAdsByOwner.Result> GetAdsByOwner(
            this IAsyncDocumentSession session, V1.GetAdsByOwner query, CancellationToken cancellationToken)
        {
            var doc = await session.LoadAsync<Projections.ClassifiedAdsByOwnerProjection.ClassifiedAdsByOwner>(
                query.OwnerId.ToString(), cancellationToken);

            if (doc == null)
            {
                return new V1.GetAdsByOwner.Result();
            }

            var items = doc.Ads
                .Where(x => query.Status == null || x.Status.ToString() == query.Status.ToString())
                .OrderByDescending(x => x.RegisteredAt)
                .Select(x => new V1.GetAdsByOwner.Result.Item
                {
                    ClassifiedAdId = x.Id,
                    Title = x.Title,
                    Text = x.Text,
                    Price = x.Price,
                    RegisteredAt = x.RegisteredAt,
                    PublishedAt = x.PublishedAt,
                    SoldAt = x.SoldAt,
                    RemovedAt = x.RemovedAt
                }).ToArray();

            return new V1.GetAdsByOwner.Result { Items = items };
        }
    }
}
