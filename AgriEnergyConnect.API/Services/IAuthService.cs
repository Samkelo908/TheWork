using AgriEnergyConnect.API.Models;

namespace AgriEnergyConnect.API.Services
{
    public interface IAuthService
    {
        Task<AuthResponse> LoginAsync(LoginModel model);
        Task<bool> RegisterAsync(RegisterModel model);
    }
}