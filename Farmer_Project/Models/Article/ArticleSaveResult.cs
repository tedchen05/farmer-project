namespace Farmer_Project.Models.Article
{
    // 文章儲存結果
    public class ArticleSaveResult
    {
        public bool Success { get; set; }
        public bool NotFound { get; set; }
        public string? ErrorField { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
