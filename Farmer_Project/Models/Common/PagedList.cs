namespace Farmer_Project.Models.Common
{
    // 分頁結果
    public class PagedList<T>
    {
        public List<T> Items { get; set; } = new List<T>();
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }

        public int TotalPages => PageSize > 0 ? (int)Math.Ceiling(TotalCount / (double)PageSize) : 0;
        public bool HasPrevious => PageNumber > 1;
        public bool HasNext => PageNumber < TotalPages;

        // 可選的每頁筆數
        public static readonly int[] AllowedPageSizes = { 5, 10, 50, 100 };

        // 將每頁筆數限制在允許範圍，否則回傳預設10
        public static int NormalizePageSize(int size)
        {
            return AllowedPageSizes.Contains(size) ? size : 10;
        }
    }
}
