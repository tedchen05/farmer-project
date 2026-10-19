using Farmer_Project.Entities;
using Farmer_Project.Models.Common;

namespace Farmer_Project.Models.Farmer
{
    // 小農列表頁(含分頁與搜尋)
    public class FarmerListViewModel
    {
        public PagedList<FarmersInfo> Farmers { get; set; } = new PagedList<FarmersInfo>();
        public string? SearchTerm { get; set; }
    }
}
