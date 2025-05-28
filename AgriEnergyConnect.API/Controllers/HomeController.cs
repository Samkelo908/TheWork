using Microsoft.AspNetCore.Mvc;

namespace AgriEnergyConnect.API.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
