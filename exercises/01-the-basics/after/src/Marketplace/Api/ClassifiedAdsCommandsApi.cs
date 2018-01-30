using Microsoft.AspNetCore.Mvc;

namespace Marketplace
{
    [Route("/ad")]
    public class ClassifiedAdsCommandsApi : Controller
    {
        private readonly ClassifiedAdsApplicationService _appService;

        public ClassifiedAdsCommandsApi(ClassifiedAdsApplicationService appService)
        {
            _appService = appService;
        }

        /// <summary>
        ///     Create a new classified ad
        /// </summary>
        [HttpPost]
        public void Post(Contracts.ClassifiedAds.V1.Create request) =>
            _appService.Handle(request);
    }
}
