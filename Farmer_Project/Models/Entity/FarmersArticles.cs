using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Farmer_Project.Models.Entity
{
    public class FarmersArticles
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ArticlesId { set; get; }

        [ForeignKey("FarmersInfo")]
        public int? FarmersId { set; get; }
    
        [StringLength(15)]
        public string ArticleType { set; get; }

        [MaxLength(100)]
        public string ArticleTitle { get; set; }

        [StringLength(50)]
        public string ArticleImagePath { set; get; }

        public string ArticleSummary { set; get; }

        public bool IsPublished { set; get; } = false;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        //導覽屬性(一對"多")
        public virtual FarmersInfo FarmersInfo { get; set; }

        //導覽屬性("一"對多)
        public virtual List<FarmersArticlesDetails> FarmersArticlesDetails { get; set; }
    }
}
