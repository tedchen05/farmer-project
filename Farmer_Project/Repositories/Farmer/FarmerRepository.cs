using Farmer_Project.Entities;
using Farmer_Project.Models.Common;
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

        // 依關鍵字搜尋小農並分頁，空字串回傳全部
        public async Task<PagedList<FarmersInfo>> SearchPagedAsync(string? search, string? transSearch, int pageNumber, int pageSize)
        {
            var query = _dbContext.FarmersInfo.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                var trans = transSearch ?? search;
                query = query.Where(f => f.Name.Contains(search) ||
                                         f.FarmName.Contains(search) ||
                                         f.CropsType.Contains(trans) ||
                                         f.Address.Contains(search) ||
                                         f.Phone.Contains(search));
            }

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(f => f.FarmersId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedList<FarmersInfo>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
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
