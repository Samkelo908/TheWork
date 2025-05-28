using AgriEnergyConnect.Web.Models;
using Newtonsoft.Json;
using System.Text;

public class AuthService
{
    private readonly HttpClient _httpClient;

    public AuthService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<AuthResponse> LoginAsync(LoginModel model)
    {
        var json = JsonConvert.SerializeObject(model);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Use correct endpoint path based on your chosen solution
        var response = await _httpClient.PostAsync("api/Auth/login", content);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"Login failed: {errorContent}");
        }

        return JsonConvert.DeserializeObject<AuthResponse>(await response.Content.ReadAsStringAsync());
    }

    public async Task RegisterAsync(RegisterModel model)
    {
        var json = JsonConvert.SerializeObject(model);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("api/Auth/register", content);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Registration failed: {error}");
        }
    }
}