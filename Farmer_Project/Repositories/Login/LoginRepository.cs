using Farmer_Project.Entities;
using Microsoft.EntityFrameworkCore;

namespace Farmer_Project.Repositories.Login
{
    public class LoginRepository : ILoginRepository
    {
        private readonly AppDbContext _dbContext;

        public LoginRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // 依帳號取得會員，查無回傳null
        public async Task<MemberInformation?> GetByAccountAsync(string account)
        {
            return await _dbContext.MemberInformations.FirstOrDefaultAsync(m => m.Account == account);
        }

        // 確認帳號是否已存在
        public async Task<bool> ExistsByAccountAsync(string account)
        {
            return await _dbContext.MemberInformations.AnyAsync(m => m.Account == account);
        }

        // 新增會員
        public void Add(MemberInformation member)
        {
            _dbContext.MemberInformations.Add(member);
        }

        // 儲存變更
        public async Task SaveAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
