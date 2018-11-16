using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using static Marketplace.Contracts.ClassifiedAds;

namespace Marketplace.Modules.ClassifiedAds
{

    [Route("/ads")]
    public class ClassifiedAdsQueriesApi : Controller
    {
        static readonly ILogger Log = Serilog.Log.ForContext<ClassifiedAdsQueriesApi>();

        ClassifiedAdsQueryService Service { get; }

        public ClassifiedAdsQueriesApi(ClassifiedAdsQueryService service) => Service = service;

        [HttpGet, Route("available")]
        public Task<IActionResult> When([FromQuery] V1.GetAvailableAds qry)
            => RunQuery(qry, () => Service.GetAvailableAds(qry, HttpContext.RequestAborted));

        async Task<IActionResult> RunQuery<T, TResult>(T query, Func<Task<TResult>> runQuery)
        {
            Log.Information(query.ToString());
            var result = await runQuery();
            return Ok(result);
        }
    }
}
