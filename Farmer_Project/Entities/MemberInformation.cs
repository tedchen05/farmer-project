using System.ComponentModel.DataAnnotations;

namespace Farmer_Project.Entities
{
    public class MemberInformation
    {
        [Key]
        public int Id { get; set; }

        [StringLength(50)]
        [Required] // 必填欄位
        public string Sort { get; set; } = string.Empty; // 預設值為空字串

        [StringLength(50)]
        [Required] // 必填欄位
        public string NickName { get; set; } = string.Empty; // 預設值為空字串

        [StringLength(50)]
        [Required] // 必填欄位
        public string Account { get; set; } = string.Empty; // 預設值為空字串

        [StringLength(50)]
        [Required] // 必填欄位
        public string Passwd { get; set; } = string.Empty; // 預設值為空字串
    }
}
