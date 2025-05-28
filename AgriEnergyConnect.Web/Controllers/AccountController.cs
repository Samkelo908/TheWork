using AgriEnergyConnect.Web.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AgriEnergyConnect.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly AuthService _authService;
        private readonly IConfiguration _configuration;

        public AccountController(AuthService authService, IConfiguration configuration)
        {
            _authService = authService;
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid) return View(model);

            try
            {
                var response = await _authService.LoginAsync(model);

                // Store token in HTTP-only cookie
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = _configuration.GetValue<bool>("UseHttps"),
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.Now.AddHours(1)
                };
                Response.Cookies.Append("access_token", response.Token, cookieOptions);

                // Create claims identity
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, response.UserId),
                    new Claim(ClaimTypes.Name, model.Email),
                    new Claim(ClaimTypes.Role, response.Role),
                    new Claim("Token", response.Token) // Store token in claims if needed
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal,
                    new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1)
                    });

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (!ModelState.IsValid) return View(model);

            try
            {
                await _authService.RegisterAsync(model);
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }

        public async Task<IActionResult> Logout()
        {
            // Clear authentication cookie
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Clear JWT token cookie
            Response.Cookies.Delete("access_token", new CookieOptions
            {
                HttpOnly = true,
                Secure = _configuration.GetValue<bool>("UseHttps"),
                SameSite = SameSiteMode.Strict
            });

            return RedirectToAction("Login");
        }
    }
}