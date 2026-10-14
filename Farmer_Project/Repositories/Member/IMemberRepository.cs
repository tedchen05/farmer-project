using Farmer_Project.Entities;

namespace Farmer_Project.Repositories.Member
{
    // 會員資料存取
    public interface IMemberRepository
    {
        List<MemberInformation> GetAll();
        List<MemberInformation> Search(string? keyword);
        MemberInformation? GetById(int id);
        void Add(MemberInformation member);
        void Remove(MemberInformation member);
        void Save();
    }
}
