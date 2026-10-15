using Farmer_Project.Entities;
using Microsoft.EntityFrameworkCore;

namespace Farmer_Project.Repositories.Farmer
{
    public class FarmerRepository : IFarmerRepository
    {
        private readonly AppDbContext _dbContext;

        public FarmerRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // 取得所有小農(含文章)
        public List<FarmersInfo> GetAllWithArticles()
        {
            return _dbContext.FarmersInfo
                .Include(f => f.FarmersArticles)
                .ToList();
        }

        // 依關鍵字搜尋小農
        public List<FarmersInfo> Search(string search, string transSearch)
        {
            return _dbContext.FarmersInfo
                .Where(f => f.Name.Contains(search) ||
                            f.FarmName.Contains(search) ||
                            f.CropsType.Contains(transSearch) ||
                            f.Address.Contains(search) ||
                            f.Phone.Contains(search))
                .Include(f => f.FarmersArticles)
                .ToList();
        }

        // 依Id取得小農
        public FarmersInfo? GetById(int id)
        {
            return _dbContext.FarmersInfo.FirstOrDefault(f => f.FarmersId == id);
        }

        // 新增小農
        public void Add(FarmersInfo farmer)
        {
            _dbContext.FarmersInfo.Add(farmer);
        }

        // 刪除小農
        public void Remove(FarmersInfo farmer)
        {
            _dbContext.FarmersInfo.Remove(farmer);
        }

        // 儲存變更
        public void Save()
        {
            _dbContext.SaveChanges();
        }
    }
}
