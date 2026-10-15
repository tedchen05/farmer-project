using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Farmer_Project.Entities
{
    public class FarmersArticlesDetails
    {
        
        [Required]
        [ForeignKey("FarmersArticles")]
        public int ArticlesId { set; get; }

        [Required]
        public int DetailId { set; get; }

        [MaxLength(100)]
        public string SubTitle { get; set; } = string.Empty;

        [StringLength(50)]
        public string SubImagePath { set; get; } = string.Empty;

        public string SubContent { set; get; } = string.Empty;

        //導覽屬性(一對"多")
        public virtual FarmersArticles FarmersArticles { get; set; } = null!;
    }
}
