using Farmer_Project.Entities;

namespace Farmer_Project.Repositories.Login
{
    // 登入/會員資料存取
    public interface ILoginRepository
    {
        MemberInformation? GetByAccount(string account);
        bool ExistsByAccount(string account);
        void Add(MemberInformation member);
        void Save();
    }
}
