using Farmer_Project.Entities;
using Farmer_Project.Models.Member;

namespace Farmer_Project.Services.Member
{
    // 會員管理商業邏輯
    public interface IMemberService
    {
        List<MemberInformation> Search(string? keyword);
        MemberInformation? GetById(int id);
        MemberResult Create(string sort, string nickName, string account, string passwd);
        MemberResult Update(int id, string sort, string nickName, string account, string passwd);
        void Delete(int id);
    }
}
