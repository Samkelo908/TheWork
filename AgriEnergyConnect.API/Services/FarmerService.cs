using AgriEnergyConnect.API.Data;
using AgriEnergyConnect.API.Models;
using Microsoft.EntityFrameworkCore;

namespace AgriEnergyConnect.API.Services
{
    public class FarmerService : IFarmerService
    {
        private readonly AgriEnergyDbContext _context;

        public FarmerService(AgriEnergyDbContext context)
        {
            _context = context;
        }

        public async Task<List<FarmerModel>> GetAllFarmersAsync()
        {
            return await _context.Farmers.ToListAsync();
        }

        public async Task<FarmerModel> GetFarmerByIdAsync(string id)
        {
            return await _context.Farmers.FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<FarmerModel> CreateFarmerAsync(FarmerModel model)
        {
            _context.Farmers.Add(model);
            await _context.SaveChangesAsync();
            return model;
        }

    }
}