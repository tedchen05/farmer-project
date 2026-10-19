using Farmer_Project.Entities;
using Farmer_Project.Models.Common;

namespace Farmer_Project.Repositories.Member
{
    // 會員資料存取
    public interface IMemberRepository
    {
        Task<List<MemberInformation>> GetAllAsync();
        Task<PagedList<MemberInformation>> SearchPagedAsync(string? keyword, int pageNumber, int pageSize);
        Task<MemberInformation?> GetByIdAsync(int id);
        void Add(MemberInformation member);
        void Remove(MemberInformation member);
        Task SaveAsync();
    }
}
