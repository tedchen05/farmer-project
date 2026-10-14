using Farmer_Project.Entities;

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
        public MemberInformation? GetByAccount(string account)
        {
            return _dbContext.MemberInformations.FirstOrDefault(m => m.Account == account);
        }

        // 確認帳號是否已存在
        public bool ExistsByAccount(string account)
        {
            return _dbContext.MemberInformations.Any(m => m.Account == account);
        }

        // 新增會員
        public void Add(MemberInformation member)
        {
            _dbContext.MemberInformations.Add(member);
        }

        // 儲存變更
        public void Save()
        {
            _dbContext.SaveChanges();
        }
    }
}
