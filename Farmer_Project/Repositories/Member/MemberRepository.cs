using Farmer_Project.Entities;

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
        public List<MemberInformation> GetAll()
        {
            return _dbContext.MemberInformations.ToList();
        }

        // 依關鍵字搜尋會員，空字串回傳全部
        public List<MemberInformation> Search(string? keyword)
        {
            var query = _dbContext.MemberInformations.AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(m => m.NickName.Contains(keyword) ||
                                         m.Account.Contains(keyword) ||
                                         m.Sort.Contains(keyword));
            }

            return query.ToList();
        }

        // 依Id取得會員
        public MemberInformation? GetById(int id)
        {
            return _dbContext.MemberInformations.FirstOrDefault(m => m.Id == id);
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
        public void Save()
        {
            _dbContext.SaveChanges();
        }
    }
}
