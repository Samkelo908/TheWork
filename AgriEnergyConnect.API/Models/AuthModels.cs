namespace AgriEnergyConnect.API.Models
{
    public class AuthResponse
    {
        public string Token { get; set; }
        public string UserId { get; set; }
        public string Role { get; set; }
    }

    public class LoginModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class RegisterModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Role { get; set; } = "Farmer"; // Default role
    }
}