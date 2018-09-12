using System;
using System.Threading.Tasks;
using Marketplace.Domain.ClassifiedAds;
using Microsoft.AspNetCore.Mvc;

namespace Marketplace.Modules.ClassifiedAds
{
    [Route("/ad")]
    public class ClassifiedAdsCommandsApi : Controller
    {
        private static readonly Serilog.ILogger Log = Serilog.Log.ForContext<ClassifiedAdsCommandsApi>();

        private readonly ClassifiedAdsApplicationService _appService;

        public ClassifiedAdsCommandsApi(ClassifiedAdsApplicationService appService) => _appService = appService;

        /// <summary>
        ///     Create a new classified ad
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Post(Contracts.ClassifiedAds.V1.CreateAd request)
        {
            Log.Information(request.ToString());
            await _appService.Handle(request);
            return Ok();
        }

        /// <summary>
        ///    Rename a classified ad
        /// </summary>
        [Route("name")]
        [HttpPut]
        public Task<IActionResult> Put(Contracts.ClassifiedAds.V1.RenameAd request) =>
            HandleOrThrow(request, x => _appService.Handle(x));

        [Route("text")]
        [HttpPut]
        public Task<IActionResult> Put(Contracts.ClassifiedAds.V1.UpdateText request) =>
            HandleOrThrow(request, _appService.Handle);

        [Route("price")]
        [HttpPut]
        public Task<IActionResult> Put(Contracts.ClassifiedAds.V1.ChangePrice request) =>
            HandleOrThrow(request, x => _appService.Handle(x));

        [Route("publish")]
        [HttpPut]
        public Task<IActionResult> Put(Contracts.ClassifiedAds.V1.Publish request) =>
            HandleOrThrow(request, x => _appService.Handle(x));

        [Route("activate")]
        [HttpPut]
        public Task<IActionResult> Put(Contracts.ClassifiedAds.V1.Activate request) =>
            HandleOrThrow(request, x => _appService.Handle(x));

        [Route("deactivate")]
        [HttpPut]
        public Task<IActionResult> Put(Contracts.ClassifiedAds.V1.Deactivate request) =>
            HandleOrThrow(request, x => _appService.Handle(x));

        [Route("report")]
        [HttpPut]
        public Task<IActionResult> Put(Contracts.ClassifiedAds.V1.Report request) =>
            HandleOrThrow(request, x => _appService.Handle(x));

        [Route("reject")]
        [HttpPut]
        public Task<IActionResult> Put(Contracts.ClassifiedAds.V1.Reject request) =>
            HandleOrThrow(request, x => _appService.Handle(x));

        [Route("marksold")]
        [HttpPut]
        public Task<IActionResult> Put(Contracts.ClassifiedAds.V1.MarkAsSold request) =>
            HandleOrThrow(request, x => _appService.Handle(x));

        [HttpDelete]
        public Task<IActionResult> Put(Contracts.ClassifiedAds.V1.Remove request) =>
            HandleOrThrow(request, x => _appService.Handle(x));

        private async Task<IActionResult> HandleOrThrow<T>(T request, Func<T, Task> handler)
        {
            try
            {
                Log.Information(request.ToString());
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
