using AgriEnergyConnect.API.Models;
using AgriEnergyConnect.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace AgriEnergyConnect.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            try
            {
                var authResponse = await _authService.LoginAsync(model);
                return Ok(authResponse);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            try
            {
                var result = await _authService.RegisterAsync(model);
                return Ok(new { Message = "User created successfully!" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}