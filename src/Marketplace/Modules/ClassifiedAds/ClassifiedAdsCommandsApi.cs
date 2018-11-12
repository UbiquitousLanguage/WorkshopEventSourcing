using System.Threading.Tasks;
using Marketplace.Domain.ClassifiedAds;
using Microsoft.AspNetCore.Mvc;
using static Marketplace.Contracts.ClassifiedAds;

namespace Marketplace.Modules.ClassifiedAds
{
    [Route("/ad")]
    public class ClassifiedAdsCommandsApi : Controller
    {
        private static readonly Serilog.ILogger Log = Serilog.Log.ForContext<ClassifiedAdsCommandsApi>();

        private readonly ClassifiedAdsApplicationService _service;

        public ClassifiedAdsCommandsApi(ClassifiedAdsApplicationService service) => _service = service;

        [HttpPost]
        public Task<IActionResult> When(V1.RegisterAd cmd) => Handle(cmd);

        [HttpPut, Route("name")]
        public Task<IActionResult> When(V1.ChangeTitle cmd) => Handle(cmd);

        [HttpPut, Route("text")]
        public Task<IActionResult> When(V1.ChangeText cmd) => Handle(cmd);

        [HttpPut, Route("price")]
        public Task<IActionResult> When(V1.ChangePrice cmd) => Handle(cmd);

        [HttpPut, Route("publish")]
        public Task<IActionResult> When(V1.Publish cmd) => Handle(cmd);

        [HttpPut, Route("mark-sold")]
        public Task<IActionResult> When(V1.MarkAsSold cmd) => Handle(cmd);

        [HttpDelete]
        public Task<IActionResult> When(V1.Remove cmd) => Handle(cmd);

        private async Task<IActionResult> Handle<T>(T cmd) where T : class
        {
            try
            {
                Log.Information(cmd.ToString());
                await _service.Handle(cmd);
                return Ok();
            }
            catch (Exceptions.ClassifiedAdNotFoundException ex)
            {
                Log.Debug(ex.ToString());
                return NotFound();
            }
        }
    }
}
