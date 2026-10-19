using Farmer_Project.Entities;
using Farmer_Project.Models.Common;

namespace Farmer_Project.Models.Member
{
    // 會員列表頁(含分頁與搜尋)
    public class MemberListViewModel
    {
        public PagedList<MemberInformation> Members { get; set; } = new PagedList<MemberInformation>();
        public string? SearchTerm { get; set; }
    }
}
