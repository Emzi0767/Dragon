using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Dragon.Controllers
{
    [Route("")]
    public class HomeController : DragonController
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [Route("")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("privacy")]
        public IActionResult Privacy()
        {
            return View();
        }
    }
}
