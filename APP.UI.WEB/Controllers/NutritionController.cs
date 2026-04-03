using Microsoft.AspNetCore.Mvc;

namespace APP.UI.WEB.Controllers
{
    public class NutritionController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
