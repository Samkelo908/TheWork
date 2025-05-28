using AgriEnergyConnect.Web.Models;
using Microsoft.AspNetCore.Authentication;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

public class EmployeeService
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly ILogger<EmployeeService> _logger;

    public EmployeeService(HttpClient httpClient, IHttpContextAccessor contextAccessor, ILogger<EmployeeService> logger)
    {
        _httpClient = httpClient;
        _contextAccessor = contextAccessor;
        _logger = logger;
    }
    private async Task AddAuthorizationHeaderAsync()
    {
        var token = _contextAccessor.HttpContext?.Request.Cookies["access_token"] ??
                    await _contextAccessor.HttpContext?.GetTokenAsync("access_token");

        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }
    }

    public async Task CreateFarmerAsync(CreateFarmerModel model)
    {
        await AddAuthorizationHeaderAsync();

        var json = JsonConvert.SerializeObject(model);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("api/Farmers", content);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception("Failed to create farmer: " + error);
        }
    }

    public async Task<List<ProductModel>> GetFilteredProductsAsync(string category, DateTime? startDate, DateTime? endDate)
    {
        try
        {
            await AddAuthorizationHeaderAsync();

            var queryParams = new List<string>();

            if (!string.IsNullOrEmpty(category))
                queryParams.Add($"category={Uri.EscapeDataString(category)}");

            if (startDate.HasValue)
                queryParams.Add($"startDate={Uri.EscapeDataString(startDate.Value.ToString("yyyy-MM-dd"))}");

            if (endDate.HasValue)
                queryParams.Add($"endDate={Uri.EscapeDataString(endDate.Value.ToString("yyyy-MM-dd"))}");

            var requestUrl = $"api/Products/filter-all?{string.Join("&", queryParams)}";

            _logger.LogInformation($"Making request to: {requestUrl}");

            var response = await _httpClient.GetAsync(requestUrl);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError($"API Error: {response.StatusCode} - {errorContent}");
                throw new HttpRequestException($"API Error: {response.StatusCode} - {errorContent}");
            }

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<ProductModel>>(content) ?? new List<ProductModel>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetFilteredProductsAsync");
            throw;
        }
    }
}
