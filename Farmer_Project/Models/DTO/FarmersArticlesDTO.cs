using System.ComponentModel.DataAnnotations;

namespace Farmer_Project.Models.DTO
{
    public class FarmersArticlesDTO
    {
        public int FarmersId { get; set; }
        public string ArticleType { get; set; }
        public string ArticleTitle { get; set; }
        public string ArticleImagePath { get; set; } // 存圖片路徑
        public string ArticleSummary { get; set; }
        public List<FarmersArticlesDetailsDTO> ArticleDetails { get; set; }
    }

    public class FarmersArticlesDetailsDTO
    {
        public string SubTitle { get; set; }
        public string SubImagePath { get; set; } // 存圖片路徑
        public string SubContent { get; set; }
    }
}
