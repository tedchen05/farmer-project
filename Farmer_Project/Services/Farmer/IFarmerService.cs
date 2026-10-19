using Farmer_Project.Entities;
using Farmer_Project.Models.Common;
using Farmer_Project.Models.Farmer;

namespace Farmer_Project.Services.Farmer
{
    // 小農管理商業邏輯
    public interface IFarmerService
    {
        Task<FarmersAndMembersViewModel> GetDashboardAsync();
        Task<PagedList<FarmersInfo>> GetFarmerListAsync(string? search, int pageNumber, int pageSize);
        Task<List<FarmersArticles>> GetFarmerIntrosAsync(string cropsType);
        Task<FarmersArticles?> GetFarmerStoryAsync(int farmersId, int articlesId);
        Task<FarmersInfo?> GetByIdAsync(int id);
        Task<FarmerSaveResult> AddAsync(FarmersInfo farmer, IFormFile? image);
        Task<FarmerSaveResult> EditAsync(int id, FarmersInfo model, IFormFile? image);
        Task<bool> DeleteAsync(int deleteFarmersId);
    }
}
