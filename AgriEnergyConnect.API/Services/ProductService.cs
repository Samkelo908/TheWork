using AgriEnergyConnect.API.Data;
using AgriEnergyConnect.API.Models;
using Microsoft.EntityFrameworkCore;

namespace AgriEnergyConnect.API.Services
{
    public class ProductService : IProductService
    {
        private readonly AgriEnergyDbContext _context;
        private readonly ILogger<ProductService> _logger;

        public ProductService(AgriEnergyDbContext context, ILogger<ProductService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> FarmerExistsAsync(string farmerId)
        {
            try
            {
                return await _context.Farmers.AnyAsync(f => f.Id == farmerId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if farmer {FarmerId} exists", farmerId);
                throw;
            }
        }
        public async Task<List<ProductModel>> GetFilteredProductsByFarmerAsync(
    string farmerId,
    string? productType,
    DateTime? startDate,
    DateTime? endDate)
        {
           
            if (string.IsNullOrEmpty(farmerId))
                throw new ArgumentException("Farmer ID is required", nameof(farmerId));


            var query = _context.Products.Where(p => p.FarmerId == farmerId);

            
            if (!string.IsNullOrEmpty(productType))
                query = query.Where(p => p.Category.ToLower().Contains(productType.ToLower()));

            if (startDate.HasValue)
                query = query.Where(p => p.HarvestDate >= startDate.Value.Date);

            if (endDate.HasValue)
                query = query.Where(p => p.HarvestDate <= endDate.Value.Date.AddDays(1).AddTicks(-1));

            return await query.ToListAsync();
        }



        public async Task<List<ProductModel>> GetProductsByFarmerAsync(string farmerId)
        {
            try
            {
                _logger.LogInformation("Attempting to get products for farmer {FarmerId}", farmerId);
                var products = await _context.Products
                    .Where(p => p.FarmerId == farmerId)
                    .ToListAsync();

                _logger.LogInformation("Found {Count} products for farmer {FarmerId}", products.Count, farmerId);
                return products;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting products for farmer {FarmerId}. Error: {ErrorMessage}",
                    farmerId, ex.Message);
                throw;
            }
        }

        public async Task<ProductModel> GetProductByIdAsync(int id)
        {
            try
            {
                return await _context.Products.FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting product with ID {ProductId}", id);
                throw;
            }
        }
        public async Task<ProductModel> CreateProductAsync(CreateProductModel model, string farmerId)
        {
            try
            {
                var product = new ProductModel
                {
                    Name = model.Name,
                    Description = model.Description,
                    Price = model.Price,
                    Quantity = model.Quantity,
                    Category = model.Category,
                    HarvestDate = model.HarvestDate,
                    FarmerId = farmerId
                };


                if (string.IsNullOrEmpty(product.Name) ||
                    string.IsNullOrEmpty(product.Description) ||
                    string.IsNullOrEmpty(product.Category) ||
                    string.IsNullOrEmpty(product.FarmerId))
                {
                    throw new ArgumentException("Required product fields are missing");
                }


                var farmerExists = await _context.Farmers.AnyAsync(f => f.Id == farmerId);
                if (!farmerExists)
                {
                    throw new KeyNotFoundException($"Farmer with ID {farmerId} not found");
                }

                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                return product;
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database error creating product");
                throw new Exception("Could not save product to database. Please verify all required fields.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product for farmer {FarmerId}", farmerId);
                throw;
            }
        }
        public async Task<ProductModel> UpdateProductAsync(int id, UpdateProductModel model, string farmerId)
        {
            try
            {
                var existingProduct = await _context.Products
                    .FirstOrDefaultAsync(p => p.Id == id && p.FarmerId == farmerId);

                if (existingProduct == null)
                {
                    throw new KeyNotFoundException($"Product with ID {id} not found or doesn't belong to farmer");
                }

                // Update properties
                existingProduct.Name = model.Name;
                existingProduct.Description = model.Description;
                existingProduct.Price = model.Price;
                existingProduct.Quantity = model.Quantity;
                existingProduct.Category = model.Category;
                existingProduct.HarvestDate = model.HarvestDate;

                
                if (string.IsNullOrEmpty(existingProduct.Name) ||
                    string.IsNullOrEmpty(existingProduct.Description) ||
                    string.IsNullOrEmpty(existingProduct.Category))
                {
                    throw new ArgumentException("Required product fields are missing");
                }

                await _context.SaveChangesAsync();
                return existingProduct;
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database error updating product {ProductId}", id);
                throw new Exception("Could not update product in database.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product {ProductId} for farmer {FarmerId}", id, farmerId);
                throw;
            }
        }

        public async Task<bool> DeleteProductAsync(int id, string farmerId)
        {
            try
            {
                var product = await _context.Products
                    .FirstOrDefaultAsync(p => p.Id == id && p.FarmerId == farmerId);

                if (product == null)
                {
                    throw new KeyNotFoundException($"Product with ID {id} not found or doesn't belong to farmer");
                }

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database error deleting product {ProductId}", id);
                throw new Exception("Could not delete product from database.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product {ProductId} for farmer {FarmerId}", id, farmerId);
                throw;
            }
        }

        public async Task<List<ProductModel>> GetFilteredProductsAsync(
            string farmerId,
            string? category,
            
            DateTime? start,
            DateTime? end)
        {
            try
            {
                if (string.IsNullOrEmpty(farmerId))
                    throw new ArgumentException("Farmer ID is required");

                var query = _context.Products.Where(p => p.FarmerId == farmerId);

                if (!string.IsNullOrEmpty(category))
                    query = query.Where(p => p.Category.ToLower().Contains(category.ToLower()));

                if (start.HasValue)
                    query = query.Where(p => p.HarvestDate >= start.Value.Date);

                if (end.HasValue)
                    query = query.Where(p => p.HarvestDate <= end.Value.Date.AddDays(1).AddTicks(-1));

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetFilteredProductsAsync");
                throw;
            }
        }

        public async Task<List<ProductModel>> FilterProductsAsync(string? category, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                // Base query with explicit select to avoid any navigation property issues
                var query = _context.Products.AsQueryable();

                
                if (!string.IsNullOrWhiteSpace(category))
                {
                    query = query.Where(p => p.Category.ToLower() == category.Trim().ToLower());
                }

                
                query = query.Where(p => p.HarvestDate.Year > 1); 

                if (startDate.HasValue)
                {
                    query = query.Where(p => p.HarvestDate >= startDate.Value);
                }

                if (endDate.HasValue)
                {
                    query = query.Where(p => p.HarvestDate <= endDate.Value);
                }

                // Additional data quality filters based on your screenshot
                query = query.Where(p =>
                    p.Quantity >= 0 &&                
                    p.Quantity < int.MaxValue &&      
                    !string.IsNullOrWhiteSpace(p.Description) && 
                    p.Price >= 0                     
                );

                // Execute with explicit ordering
                var result = await query
                    .OrderBy(p => p.HarvestDate)
                    .ThenBy(p => p.Category)
                    .ToListAsync();

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error filtering products. Category: {Category}, Start: {Start}, End: {End}",
                    category, startDate, endDate);

                // Return empty list or throw based on your error handling policy
                return new List<ProductModel>();
            }
        }

    }
}


