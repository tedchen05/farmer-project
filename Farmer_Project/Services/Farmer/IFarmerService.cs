using Farmer_Project.Entities;
using Farmer_Project.Models.Farmer;

namespace Farmer_Project.Services.Farmer
{
    // 小農管理商業邏輯
    public interface IFarmerService
    {
        FarmersAndMembersViewModel GetDashboard();
        FarmersAndMembersViewModel Search(string search);
        List<FarmersArticles> GetFarmerIntros(string cropsType);
        FarmersArticles? GetFarmerStory(int farmersId, int articlesId);
        FarmersInfo? GetById(int id);
        Task<FarmerSaveResult> AddAsync(FarmersInfo farmer, IFormFile? image);
        Task<FarmerSaveResult> EditAsync(int id, FarmersInfo model, IFormFile? image);
        bool Delete(int deleteFarmersId);
    }
}
