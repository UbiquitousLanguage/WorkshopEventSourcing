using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Raven.Client.Documents.Session;
using Serilog;
using static Marketplace.Contracts.ClassifiedAds;

namespace Marketplace.Modules.ClassifiedAds
{
    [Route("/ad")]
    public class ClassifiedAdsQueriesApi : Controller
    {
        private static readonly ILogger Log = Serilog.Log.ForContext<ClassifiedAdsQueriesApi>();

        private Func<IAsyncDocumentSession> GetSession { get; }

        public ClassifiedAdsQueriesApi(Func<IAsyncDocumentSession> getSession) => GetSession = getSession;

        [HttpGet] public Task<IActionResult> When([FromQuery] V1.GetAvailableAds qry) => RunQuery(qry, session => session.GetAvailableAds(qry));

        private async Task<IActionResult> RunQuery<T, TResult>(T query, Func<IAsyncDocumentSession, Task<TResult>> runQuery)
            where T : class where TResult : class
        {
            Log.Information(query.ToString());
            using (var session = GetSession())
            {
                var result = await runQuery(session);
                return Ok(result);
            }
        }
    }
}
