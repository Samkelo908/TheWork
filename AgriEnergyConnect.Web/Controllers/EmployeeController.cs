using AgriEnergyConnect.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgriEnergyConnect.Web.Controllers
{
    [Authorize(Roles = "Employee")]
    public class EmployeeController : Controller
    {
        private readonly EmployeeService _employeeService;
        private readonly ILogger<EmployeeController> _logger;

        public EmployeeController(EmployeeService employeeService, ILogger<EmployeeController> logger)
        {
            _employeeService = employeeService;
            _logger = logger;
        }

        // 1. Show form to add farmer
        public IActionResult CreateFarmer()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateFarmer(CreateFarmerModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                await _employeeService.CreateFarmerAsync(model);
                TempData["Message"] = "Farmer profile created successfully!";
                return RedirectToAction("CreateFarmer");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }

        public async Task<IActionResult> FilteredProducts(string category, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                _logger.LogInformation($"Filtering products - Category: {category}, Start: {startDate}, End: {endDate}");

                var products = await _employeeService.GetFilteredProductsAsync(category, startDate, endDate);

                _logger.LogInformation($"Found {products.Count} products matching filters");

                return View(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error filtering products");
                TempData["ErrorMessage"] = "An error occurred while filtering products: " + ex.Message;
                return View(new List<ProductModel>());
            }
        }
    }

}

