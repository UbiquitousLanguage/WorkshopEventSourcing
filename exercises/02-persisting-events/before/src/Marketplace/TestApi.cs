using System;
using Microsoft.AspNetCore.Mvc;

namespace Marketplace
{
    [Route("/test")]
    public class TestApi : Controller
    {
        [HttpGet]
        public DateTime Get() => DateTime.Now;
    }
}
