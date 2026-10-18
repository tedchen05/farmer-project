using Farmer_Project.Entities;

namespace Farmer_Project.Repositories.Member
{
    // 會員資料存取
    public interface IMemberRepository
    {
        Task<List<MemberInformation>> GetAllAsync();
        Task<List<MemberInformation>> SearchAsync(string? keyword);
        Task<MemberInformation?> GetByIdAsync(int id);
        void Add(MemberInformation member);
        void Remove(MemberInformation member);
        Task SaveAsync();
    }
}
