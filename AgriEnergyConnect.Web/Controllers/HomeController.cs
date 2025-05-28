using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgriEnergyConnect.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        [Authorize]
        public IActionResult Dashboard()
        {
            if (User.IsInRole("Farmer"))
            {
                return RedirectToAction("MyProducts", "Product");
            }
            else if (User.IsInRole("Employee") || User.IsInRole("HR"))
            {
                return RedirectToAction("Index", "Farmer");
            }

            return View();
        }
    }
}