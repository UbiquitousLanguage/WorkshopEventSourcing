using System;
using Microsoft.AspNetCore.Mvc;

namespace Marketplace.Modules
{
    [Route("/test")]
    public class TestApi : Controller
    {
        [HttpGet]
        public DateTime Get() => DateTime.Now;
    }
}
