using Farmer_Project.Entities;
using Farmer_Project.Models.Common;
using Microsoft.EntityFrameworkCore;

namespace Farmer_Project.Repositories.Member
{
    public class MemberRepository : IMemberRepository
    {
        private readonly AppDbContext _dbContext;

        public MemberRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // 取得所有會員
        public async Task<List<MemberInformation>> GetAllAsync()
        {
            return await _dbContext.MemberInformations.ToListAsync();
        }

        // 依關鍵字搜尋會員並分頁，空字串回傳全部
        public async Task<PagedList<MemberInformation>> SearchPagedAsync(string? keyword, int pageNumber, int pageSize)
        {
            var query = _dbContext.MemberInformations.AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(m => m.NickName.Contains(keyword) ||
                                         m.Account.Contains(keyword) ||
                                         m.Sort.Contains(keyword));
            }

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(m => m.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedList<MemberInformation>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        // 依Id取得會員
        public async Task<MemberInformation?> GetByIdAsync(int id)
        {
            return await _dbContext.MemberInformations.FirstOrDefaultAsync(m => m.Id == id);
        }

        // 新增會員
        public void Add(MemberInformation member)
        {
            _dbContext.MemberInformations.Add(member);
        }

        // 刪除會員
        public void Remove(MemberInformation member)
        {
            _dbContext.MemberInformations.Remove(member);
        }

        // 儲存變更
        public async Task SaveAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
