using AgriEnergyConnect.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AgriEnergyConnect.Web.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly ProductService _productService;

        public ProductController(ProductService productService)
        {
            _productService = productService;
        }

        [Authorize(Roles = "Employee,HR")]
        public async Task<IActionResult> FarmerProducts(string farmerId)
        {
            var products = await _productService.GetProductsByFarmerAsync(farmerId);
            return View(products);
        }

        [Authorize(Roles = "Farmer")]
        public async Task<IActionResult> MyProducts()
        {
            var products = await _productService.GetMyProductsAsync();
            return View(products);
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            return View(product);
        }

        [Authorize(Roles = "Farmer")]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            return View(new UpdateProductModel
            {
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Quantity = product.Quantity,
                Category = product.Category,
                HarvestDate = product.HarvestDate
            });
        }

        [Authorize(Roles = "Farmer")]
        [HttpPost]
        public async Task<IActionResult> Edit(int id, UpdateProductModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await _productService.UpdateProductAsync(id, model);
                return RedirectToAction("MyProducts");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }
        [Authorize(Roles = "Farmer")]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _productService.DeleteProductAsync(id);
                return RedirectToAction("MyProducts");
            }
            catch (Exception ex)
            {
               
                return RedirectToAction("MyProducts");
            }
        }

        [Authorize(Roles = "Farmer")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateProductModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await _productService.CreateProductAsync(model);
                return RedirectToAction("MyProducts");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }

        }
    }
}