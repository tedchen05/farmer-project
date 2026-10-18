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
        public async Task<List<FarmersInfo>> GetAllWithArticlesAsync()
        {
            return await _dbContext.FarmersInfo
                .Include(f => f.FarmersArticles)
                .ToListAsync();
        }

        // 依關鍵字搜尋小農
        public async Task<List<FarmersInfo>> SearchAsync(string search, string transSearch)
        {
            return await _dbContext.FarmersInfo
                .Where(f => f.Name.Contains(search) ||
                            f.FarmName.Contains(search) ||
                            f.CropsType.Contains(transSearch) ||
                            f.Address.Contains(search) ||
                            f.Phone.Contains(search))
                .Include(f => f.FarmersArticles)
                .ToListAsync();
        }

        // 依Id取得小農
        public async Task<FarmersInfo?> GetByIdAsync(int id)
        {
            return await _dbContext.FarmersInfo.FirstOrDefaultAsync(f => f.FarmersId == id);
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
        public async Task SaveAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
