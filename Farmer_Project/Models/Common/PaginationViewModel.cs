namespace Farmer_Project.Models.Common
{
    // 分頁列與每頁筆數選單所需資訊
    public class PaginationViewModel
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
        public string Action { get; set; } = "Index";
        public string Controller { get; set; } = string.Empty;
        public Dictionary<string, string?> RouteValues { get; set; } = new Dictionary<string, string?>();
        public int[] AllowedPageSizes { get; set; } = PagedList<object>.AllowedPageSizes;
    }
}
