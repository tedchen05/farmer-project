using Farmer_Project.Entities;

namespace Farmer_Project.Repositories.Farmer
{
    // 小農基本資料存取
    public interface IFarmerRepository
    {
        Task<List<FarmersInfo>> GetAllWithArticlesAsync();
        Task<List<FarmersInfo>> SearchAsync(string search, string transSearch);
        Task<FarmersInfo?> GetByIdAsync(int id);
        void Add(FarmersInfo farmer);
        void Remove(FarmersInfo farmer);
        Task SaveAsync();
    }
}
