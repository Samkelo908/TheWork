using AgriEnergyConnect.API.Models;

namespace AgriEnergyConnect.API.Services
{
    public interface IFarmerService
    {
        Task<List<FarmerModel>> GetAllFarmersAsync();
        Task<FarmerModel> GetFarmerByIdAsync(string id);
        Task<FarmerModel> CreateFarmerAsync(FarmerModel model);


    }
}