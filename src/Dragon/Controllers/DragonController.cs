using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Dragon.Controllers
{
    public abstract class DragonController : Controller
    {
        protected string Theme => Request.Cookies["dragon_theme"] ?? "dark";
        
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            ViewData["dragon_theme"] = Theme;
            base.OnActionExecuted(context);
        }
    }
}