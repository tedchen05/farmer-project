using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Farmer_Project.Entities
{
    public class FarmersInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FarmersId { set; get; }

        [StringLength(50)]
        [Required]
        public string Name { set; get; } = string.Empty;

        [StringLength(50)]
        [Required]
        public string FarmName { set; get; } = string.Empty;

        [StringLength(100)]
        [Required]
        public string Image { set; get; } = string.Empty;

        [StringLength(50)]
        [Required]
        [EmailAddress]
        public string Email { set; get; } = string.Empty;

        [StringLength(20)]
        [Required]
        public string Phone { set; get; } = string.Empty;

        [StringLength(50)]
        [Required]
        public string Address { set; get; } = string.Empty;

        [StringLength(20)]
        public string CropsType { set; get; } = "未選擇";

        [StringLength(20)]
        public string PlantType { set; get; } = "未選擇";

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        //導覽屬性("一"對多)
        public virtual ICollection<FarmersArticles> FarmersArticles { get; set; } = new List<FarmersArticles>();
    }
}
