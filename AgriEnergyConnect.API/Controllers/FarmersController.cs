using AgriEnergyConnect.API.Models;
using AgriEnergyConnect.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AgriEnergyConnect.API.Controllers
{
    [Authorize] 
    [Route("api/[controller]")]
    [ApiController]
    public class FarmersController : ControllerBase
    {
        private readonly IFarmerService _farmerService;

        public FarmersController(IFarmerService farmerService)
        {
            _farmerService = farmerService;
        }

        [Authorize(Roles = "Employee,HR")]
        [HttpGet]
        public async Task<IActionResult> GetAllFarmers()
        {
            var farmers = await _farmerService.GetAllFarmersAsync();
            return Ok(farmers);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetFarmerById(string id)
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            
            if (userRole != "HR" && userRole != "Employee")
            {
                if (userId != id)
                    return Forbid(); 
            }

            var farmer = await _farmerService.GetFarmerByIdAsync(id);
            if (farmer == null)
                return NotFound();

            return Ok(farmer);
        }


        [Authorize(Roles = "Employee,HR")]
        [HttpPost]
        public async Task<IActionResult> CreateFarmer([FromBody] FarmerModel model)
        {
            var createdFarmer = await _farmerService.CreateFarmerAsync(model);
            return CreatedAtAction(nameof(GetFarmerById), new { id = createdFarmer.Id }, createdFarmer);
        }
    }
}
