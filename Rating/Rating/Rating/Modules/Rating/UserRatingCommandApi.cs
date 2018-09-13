using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Rating.Modules.Rating
{
    [Route("/rating")]
    public class UserRatingCommandApi : Controller
    {
        private readonly UserRatingApplicationService _applicationService;

        public UserRatingCommandApi(UserRatingApplicationService applicationService)
            => _applicationService = applicationService;

        [HttpPost]
        public async Task<IActionResult> Post(Contracts.UserRatingContracts.V1.AddDealRate request)
        {
            Log.Information(request.ToString());
            await _applicationService.Handle(request);
            return Ok();
        }

        [HttpGet]
        public object Get()
        {
            return new {Something = 123};
        }
    }
}
