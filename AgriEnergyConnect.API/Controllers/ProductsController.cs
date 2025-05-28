using AgriEnergyConnect.API.Models;
using AgriEnergyConnect.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace AgriEnergyConnect.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(
            IProductService productService,
            ILogger<ProductsController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        [Authorize(Roles = "Employee,HR")]
        [HttpGet("filter-by-farmer")]
        public async Task<IActionResult> GetFilteredProductsByFarmer(
            [FromQuery] string farmerId,
            [FromQuery] string? productType,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            try
            {
                if (string.IsNullOrEmpty(farmerId))
                {
                    return BadRequest("Farmer ID is required");
                }

                // Verify farmer exists first
                var farmerExists = await _productService.FarmerExistsAsync(farmerId);
                if (!farmerExists)
                {
                    return NotFound($"Farmer with ID {farmerId} not found");
                }

                var products = await _productService.GetFilteredProductsByFarmerAsync(farmerId, productType, startDate, endDate);
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error filtering products for farmer {farmerId}");
                return StatusCode(500, "An error occurred while filtering products");
            }
        }

        [Authorize(Roles = "Employee,HR")]
        [HttpGet("filter-by-criteria")]
        public async Task<IActionResult> FilterProductsByCriteria(
            [FromQuery] string FarmerId,
            [FromQuery] string? category,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            try
            {
                _logger.LogInformation($"Filtering products - Type: {category}, Start: {startDate}, End: {endDate} farmerid: {FarmerId}");

                var products = await _productService.GetFilteredProductsAsync(FarmerId, category, startDate, endDate);
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error filtering products");
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Employee,HR")]
        [HttpGet("farmer/{farmerId}")]
        public async Task<IActionResult> GetProductsByFarmer(string farmerId)
        {
            try
            {
                // Verify farmer exists first
                var farmerExists = await _productService.FarmerExistsAsync(farmerId);
                if (!farmerExists)
                {
                    return NotFound($"Farmer with ID {farmerId} not found");
                }

                var products = await _productService.GetProductsByFarmerAsync(farmerId);
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting products for farmer {farmerId}");
                return StatusCode(500, "An error occurred while retrieving products");
            }
        }
        [Authorize(Roles = "Farmer")]
        [HttpGet("my-products")]
        public async Task<IActionResult> GetMyProducts()
        {
            try
            {
                var farmerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var products = await _productService.GetProductsByFarmerAsync(farmerId);
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting products for current farmer");
                return StatusCode(500, "An error occurred while retrieving your products");
            }
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                if (product == null)
                    return NotFound($"Product with ID {id} not found");

                return Ok(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting product with ID {id}");
                return StatusCode(500, $"An error occurred while retrieving product {id}");
            }
        }

        [Authorize(Roles = "Farmer")]
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var farmerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var createdProduct = await _productService.CreateProductAsync(model, farmerId);

                return CreatedAtAction(
                    nameof(GetProductById),
                    new { id = createdProduct.Id },
                    new
                    {
                        createdProduct.Id,
                        createdProduct.Name,
                        createdProduct.Price,
                        createdProduct.Category,
                        createdProduct.FarmerId

                    });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product");
                return StatusCode(500, "An error occurred while creating the product");
            }
        }

        [Authorize(Roles = "Farmer")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateProductModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var farmerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var updatedProduct = await _productService.UpdateProductAsync(id, model, farmerId);

                return Ok(new
                {
                    updatedProduct.Id,
                    updatedProduct.Name,
                    updatedProduct.Price,
                    updatedProduct.Category,
                    updatedProduct.FarmerId
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product {ProductId}", id);
                return StatusCode(500, "An error occurred while updating the product");
            }
        }



        [Authorize(Roles = "Employee,HR")]
        [HttpGet("filter-all")]
        public async Task<IActionResult> FilterAllProducts(
    [FromQuery] string? category,
    [FromQuery] DateTime? startDate,
    [FromQuery] DateTime? endDate)
        {
            try
            {
                var products = await _productService.FilterProductsAsync(category, startDate, endDate);
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error filtering all products");
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Farmer")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                var farmerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(farmerId))
                {
                    return Unauthorized("Invalid farmer identification");
                }

                var result = await _productService.DeleteProductAsync(id, farmerId);

                if (result)
                {
                    return NoContent();
                }
                return StatusCode(500, "Failed to delete product");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product {ProductId}", id);
                return StatusCode(500, "An error occurred while deleting the product");
            }
        }
    }
}