using AgriEnergyConnect.Web.Models;
using Microsoft.AspNetCore.Authentication;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

public class ProductService
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<ProductService> _logger;

    public ProductService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor, ILogger<ProductService> logger)
    {
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    private async Task AddAuthorizationHeaderAsync()
    {
        // Try to get token from cookie first
        var token = _httpContextAccessor.HttpContext?.Request.Cookies["access_token"];

        // If not in cookie, try to get from authentication context
        if (string.IsNullOrEmpty(token))
        {
            token = await _httpContextAccessor.HttpContext?.GetTokenAsync("access_token");
        }

        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }
    }


    public async Task<List<ProductModel>> GetProductsByFarmerAsync(string farmerId)
    {
        await AddAuthorizationHeaderAsync();

        var response = await _httpClient.GetAsync($"api/Products/farmer/{farmerId}");

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"Failed to get products: {errorContent}");
        }

        return JsonConvert.DeserializeObject<List<ProductModel>>(
            await response.Content.ReadAsStringAsync()) ?? new List<ProductModel>();
    }

    public async Task<List<ProductModel>> GetMyProductsAsync()
    {
        await AddAuthorizationHeaderAsync();

        var response = await _httpClient.GetAsync("api/Products/my-products");

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"Failed to get products: {errorContent}");
        }

        return JsonConvert.DeserializeObject<List<ProductModel>>(
            await response.Content.ReadAsStringAsync()) ?? new List<ProductModel>();
    }


    public async Task<ProductModel> GetProductByIdAsync(int id)
    {
        await AddAuthorizationHeaderAsync();

        var response = await _httpClient.GetAsync($"api/Products/{id}");

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"Failed to get product: {errorContent}");
        }

        return JsonConvert.DeserializeObject<ProductModel>(
            await response.Content.ReadAsStringAsync())
            ?? throw new Exception("Product data was null");
    }

    public async Task<ProductModel> CreateProductAsync(CreateProductModel model)
    {
        await AddAuthorizationHeaderAsync();

        var json = JsonConvert.SerializeObject(model);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("api/Products", content);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"Failed to create product: {errorContent}");
        }

        return JsonConvert.DeserializeObject<ProductModel>(
            await response.Content.ReadAsStringAsync())
            ?? throw new Exception("Created product data was null");
    }

public async Task<ProductModel> UpdateProductAsync(int id, UpdateProductModel model)
    {
        await AddAuthorizationHeaderAsync();

        var json = JsonConvert.SerializeObject(model);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PutAsync($"api/Products/{id}", content);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"Failed to update product: {errorContent}");
        }

        return JsonConvert.DeserializeObject<ProductModel>(
            await response.Content.ReadAsStringAsync())
            ?? throw new Exception("Updated product data was null");
    }

    public async Task DeleteProductAsync(int id)
    {
        await AddAuthorizationHeaderAsync();

        var response = await _httpClient.DeleteAsync($"api/Products/{id}");

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"Failed to delete product: {errorContent}");
        }
    }
}