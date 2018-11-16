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
    public class ClassifiedAdsQueryService
    {
        Func<IAsyncDocumentSession> GetSession { get; }

        public ClassifiedAdsQueryService(Func<IAsyncDocumentSession> getSession)
            => GetSession = getSession;

        public async Task<V1.GetAvailableAds.Result> GetAvailableAds(
            V1.GetAvailableAds query, CancellationToken cancellationToken)
        {
            using (var session = GetSession())
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
        }
    }
}
