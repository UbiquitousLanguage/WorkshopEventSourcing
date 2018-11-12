using System;
using System.Threading.Tasks;
using Raven.Client.Documents;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Session;
using static Marketplace.Contracts.ClassifiedAds;

namespace Marketplace.Modules.ClassifiedAds
{
    public static class ClassifiedAdsQueries
    {
        public static async Task<V1.GetAvailableAds.Result> GetAvailableAds(this IAsyncDocumentSession session, V1.GetAvailableAds query)
        {
            if (query.Page <= 0) query.Page = 1;
            if (query.PageSize <= 0) query.PageSize = 10;

            var result = await session.Query<Projections.AvailableClassifiedAdsProjection.AvailableClassifiedAd>()
                .Statistics(out var statistics)
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(x => new V1.GetAvailableAds.Result.Item
                {
                    ClassifiedAdId = x.ClassifiedAdId,
                    Title = x.Title,
                    Price = x.Price,
                    PublishedAt = x.PublishedAt
                })
                .ToListAsync();

            return new V1.GetAvailableAds.Result
            {
                Page = query.Page,
                PageSize = query.PageSize,
                TotalItems = statistics.TotalResults - statistics.SkippedResults,
                TotalPages = (int) Math.Ceiling((double) statistics.TotalResults / query.PageSize),
                Items = result.ToArray()
            };
        }
    }
}
