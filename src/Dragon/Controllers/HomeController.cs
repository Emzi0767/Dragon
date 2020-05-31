using Microsoft.AspNetCore.Mvc;

namespace Dragon.Controllers
{
    [Route("")]
    public class HomeController : DragonController
    {
        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("privacy")]
        public IActionResult Privacy()
        {
            return View();
        }
    }
}
