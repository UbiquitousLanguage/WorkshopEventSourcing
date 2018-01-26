using System.Threading.Tasks;
using Marketplace.Framework;
using Microsoft.AspNetCore.Mvc;

namespace Marketplace
{
    [Route("/ad")]
    public class ClassifiedAdsCommandsApi : Controller
    {
        private readonly IAggregateStore _store;
        
        public ClassifiedAdsCommandsApi(IAggregateStore store)
        {
            _store = store;
        }

        [HttpPost]
        public Task Post(Contracts.ClassifiedAds.V1.CreateClassifiedAd request) =>
            ClassifiedAdsApplicationService.Handle(request, _store);
    }
}
