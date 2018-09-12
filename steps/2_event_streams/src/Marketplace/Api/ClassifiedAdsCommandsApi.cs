using System.Threading.Tasks;
using Marketplace.Framework;
using Microsoft.AspNetCore.Mvc;

namespace Marketplace
{
    using System;
    using Domain.ClassifiedAds;

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

        /// <summary>
        ///    Rename a classified ad
        /// </summary>
        [Route("name")]
        [HttpPut]
        public Task<IActionResult> Put(Contracts.ClassifiedAds.V1.RenameAd request)
        {

        }

        [Route("text")]
        [HttpPut]
        public Task<IActionResult> Put(Contracts.ClassifiedAds.V1.UpdateText request)
        {

        }

        [Route("price")]
        [HttpPut]
        public Task<IActionResult> Put(Contracts.ClassifiedAds.V1.ChangePrice request)
        {

        }

        private async Task<IActionResult> HandleOrThrow<T>(T request, Func<T, Task> handler)
        {
            try
            {
                await handler(request);
                return Ok();
            }
            catch (Exceptions.ClassifiedAdNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
