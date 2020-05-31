using Microsoft.AspNetCore.Mvc;

namespace Dragon.Controllers
{
    [Route("[controller]")]
    public class ThemeController : DragonController
    {
        [HttpPost]
        public IActionResult ToggleTheme()
        {
            var theme = Theme == "dark" ? "light" : "dark"; 
            
            ViewData["dragon_theme"] = theme;
            Response.Cookies.Append("dragon_theme", theme);

            return Ok();
        }
    }
}