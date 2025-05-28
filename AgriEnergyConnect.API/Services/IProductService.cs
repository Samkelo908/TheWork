using AgriEnergyConnect.API.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AgriEnergyConnect.API.Services
{
    public interface IProductService
    {
        Task<bool> FarmerExistsAsync(string farmerId);
        Task<List<ProductModel>> GetProductsByFarmerAsync(string farmerId);
        Task<ProductModel> GetProductByIdAsync(int id);
        Task<ProductModel> CreateProductAsync(CreateProductModel model, string farmerId);
        Task<ProductModel> UpdateProductAsync(int id, UpdateProductModel model, string farmerId);
        Task<bool> DeleteProductAsync(int id, string farmerId);
        Task<List<ProductModel>> FilterProductsAsync(string type, DateTime? start, DateTime? end);
        Task<List<ProductModel>> GetFilteredProductsAsync(string farmerId, string category, DateTime? start, DateTime? end);

        Task<List<ProductModel>> GetFilteredProductsByFarmerAsync(
            string farmerId,
            string? productType,
            DateTime? startDate,
            DateTime? endDate);
    }
}