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

    // *************************************
    // just another option
    // *************************************

    //    [Route("/ads")]
    //    public class ClassifiedAdsQueriesApi : Controller
    //    {
    //        static readonly ILogger Log = Serilog.Log.ForContext<ClassifiedAdsQueriesApi>();
    //
    //        Func<IAsyncDocumentSession> GetSession { get; }
    //
    //        public ClassifiedAdsQueriesApi(Func<IAsyncDocumentSession> getSession) => GetSession = getSession;
    //
    //        [HttpGet, Route("available")]
    //        public Task<IActionResult> When([FromQuery] V1.GetAvailableAds qry)
    //            => RunQuery(qry, session => session.GetAvailableAds(qry, HttpContext.RequestAborted));
    //
    //        async Task<IActionResult> RunQuery<T, TResult>(T query, Func<IAsyncDocumentSession, Task<TResult>> runQuery)
    //            where T : class where TResult : class
    //        {
    //            Log.Information(query.ToString());
    //            using (var session = GetSession())
    //            {
    //                var result = await runQuery(session);
    //                return Ok(result);
    //            }
    //        }
    //    }
}
