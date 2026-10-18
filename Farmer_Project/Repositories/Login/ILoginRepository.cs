using Farmer_Project.Entities;

namespace Farmer_Project.Repositories.Login
{
    // 登入/會員資料存取
    public interface ILoginRepository
    {
        Task<MemberInformation?> GetByAccountAsync(string account);
        Task<bool> ExistsByAccountAsync(string account);
        void Add(MemberInformation member);
        Task SaveAsync();
    }
}
