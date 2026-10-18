using Farmer_Project.Entities;
using Farmer_Project.Models.Member;

namespace Farmer_Project.Services.Member
{
    // 會員管理商業邏輯
    public interface IMemberService
    {
        Task<List<MemberInformation>> SearchAsync(string? keyword);
        Task<MemberInformation?> GetByIdAsync(int id);
        Task<MemberResult> CreateAsync(string sort, string nickName, string account, string passwd);
        Task<MemberResult> UpdateAsync(int id, string sort, string nickName, string account, string passwd);
        Task DeleteAsync(int id);
    }
}
