using System.Threading.Tasks;
using Marketplace.Domain.ClassifiedAds;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using static Marketplace.Contracts.ClassifiedAds;

namespace Marketplace.Modules.ClassifiedAds
{
    [Route("/ads")]
    public class ClassifiedAdsCommandsApi : Controller
    {
        private static readonly ILogger Log = Serilog.Log.ForContext<ClassifiedAdsCommandsApi>();

        private readonly ClassifiedAdsApplicationService _service;

        public ClassifiedAdsCommandsApi(ClassifiedAdsApplicationService service) => _service = service;

        [HttpPost, Route("register")]
        public Task<IActionResult> When([FromBody] V1.Register cmd) => Handle(cmd);

        [HttpPost, Route("change-title")]
        public Task<IActionResult> When([FromBody] V1.ChangeTitle cmd) => Handle(cmd);

        [HttpPost, Route("change-text")]
        public Task<IActionResult> When([FromBody] V1.ChangeText cmd) => Handle(cmd);

        [HttpPost, Route("change-price")]
        public Task<IActionResult> When([FromBody] V1.ChangePrice cmd) => Handle(cmd);

        [HttpPost, Route("publish")]
        public Task<IActionResult> When([FromBody] V1.Publish cmd) => Handle(cmd);

        [HttpPost, Route("mark-as-sold")]
        public Task<IActionResult> When([FromBody] V1.MarkAsSold cmd) => Handle(cmd);

        [HttpPost, Route("remove")]
        public Task<IActionResult> When([FromBody] V1.Remove cmd) => Handle(cmd);

        private async Task<IActionResult> Handle<T>(T cmd) where T : class
        {
            try
            {
                Log.Information(cmd.ToString());
                await _service.Handle(cmd);
                return Ok();
            }
            catch (ClassifiedAdNotFound ex)
            {
                Log.Debug(ex.ToString());
                return NotFound();
            }
        }
    }
}
