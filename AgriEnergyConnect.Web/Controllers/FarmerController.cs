using AgriEnergyConnect.Web.Models;
//using AgriEnergyConnect.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgriEnergyConnect.Web.Controllers
{
    //[Authorize(Roles = "Employee,HR")]
    //public class FarmerController : Controller
    //{
    //    private readonly IApiService _apiService;

    //    public FarmerController(IApiService apiService)
    //    {
    //        _apiService = apiService;
    //    }

    //    public async Task<IActionResult> Index()
    //    {
    //        var farmers = await _apiService.GetAllFarmersAsync();
    //        return View(farmers);
    //    }

    //    public async Task<IActionResult> Details(string id)
    //    {
    //        var farmerProducts = new FarmerDetailsViewModel
    //        {
    //            Farmer = (await _apiService.GetAllFarmersAsync()).FirstOrDefault(f => f.Id == id),
    //            Products = await _apiService.GetProductsByFarmerAsync(id)
    //        };

    //        if (farmerProducts.Farmer == null)
    //        {
    //            return NotFound();
    //        }

    //        return View(farmerProducts);
    //    }

    //    [HttpGet]
    //    public IActionResult Create()
    //    {
    //        return View();
    //    }

    //    [HttpPost]
    //    [ValidateAntiForgeryToken]
    //    public async Task<IActionResult> Create(FarmerViewModel model)
    //    {
    //        if (!ModelState.IsValid)
    //        {
    //            return View(model);
    //        }

    //        try
    //        {
    //            var createdFarmer = await _apiService.CreateFarmerAsync(model);
    //            return RedirectToAction(nameof(Details), new { id = createdFarmer.Id });
    //        }
    //        catch
    //        {
    //            ModelState.AddModelError(string.Empty, "Error creating farmer");
    //            return View(model);
    //        }
    //    }
  //  }
}