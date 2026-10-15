using Farmer_Project.Entities;

namespace Farmer_Project.Repositories.Farmer
{
    // 小農基本資料存取
    public interface IFarmerRepository
    {
        List<FarmersInfo> GetAllWithArticles();
        List<FarmersInfo> Search(string search, string transSearch);
        FarmersInfo? GetById(int id);
        void Add(FarmersInfo farmer);
        void Remove(FarmersInfo farmer);
        void Save();
    }
}
