using Farmer_Project.Entities;
using Farmer_Project.Models.Common;

namespace Farmer_Project.Repositories.Farmer
{
    // 小農基本資料存取
    public interface IFarmerRepository
    {
        Task<List<FarmersInfo>> GetAllWithArticlesAsync();
        Task<PagedList<FarmersInfo>> SearchPagedAsync(string? search, string? transSearch, int pageNumber, int pageSize);
        Task<FarmersInfo?> GetByIdAsync(int id);
        void Add(FarmersInfo farmer);
        void Remove(FarmersInfo farmer);
        Task SaveAsync();
    }
}
