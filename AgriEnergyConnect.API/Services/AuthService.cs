using AgriEnergyConnect.API.Data;
using AgriEnergyConnect.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AgriEnergyConnect.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly AgriEnergyDbContext _context;

        public AuthService(
         UserManager<ApplicationUser> userManager,
         RoleManager<IdentityRole> roleManager,
         IConfiguration configuration,
         AgriEnergyDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _context = context;
        }
        public async Task<AuthResponse> LoginAsync(LoginModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var token = GetToken(authClaims);

                return new AuthResponse
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                    UserId = user.Id,
                    Role = userRoles.FirstOrDefault()
                };
            }
            throw new UnauthorizedAccessException("Invalid login attempt");
        }

        public async Task<bool> RegisterAsync(RegisterModel model)
        {
            var userExists = await _userManager.FindByEmailAsync(model.Email);
            if (userExists != null)
                throw new Exception("User already exists!");

            ApplicationUser user = new()
            {
                Email = model.Email,
                UserName = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
                throw new Exception($"User creation failed! {string.Join(", ", result.Errors.Select(e => e.Description))}");

            if (!await _roleManager.RoleExistsAsync(model.Role))
                await _roleManager.CreateAsync(new IdentityRole(model.Role));

            if (await _roleManager.RoleExistsAsync(model.Role))
                await _userManager.AddToRoleAsync(user, model.Role);

            // Create corresponding farmer profile for farmers
            if (model.Role == "Farmer")
            {
                await _context.Farmers.AddAsync(new FarmerModel
                {
                    Id = user.Id,
                    Name = $"{model.FirstName} {model.LastName}",
                    ContactEmail = model.Email
                });
                await _context.SaveChangesAsync();
            }

            return true;
        }

        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }
    }
}