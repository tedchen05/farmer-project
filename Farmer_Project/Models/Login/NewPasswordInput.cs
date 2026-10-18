using System.ComponentModel.DataAnnotations;

namespace Farmer_Project.Models.Login
{
    // 忘記密碼:重設密碼輸入
    public class NewPasswordInput
    {
        [Required(ErrorMessage = "請輸入新密碼")]
        [StringLength(12, MinimumLength = 8, ErrorMessage = "密碼需要8~12位數")]
        [Display(Name = "新密碼")]
        public string Passwd { get; set; } = string.Empty;

        [Required(ErrorMessage = "請再次輸入新密碼")]
        [Compare(nameof(Passwd), ErrorMessage = "兩次輸入的密碼不一致")]
        [Display(Name = "再次輸入新密碼")]
        public string Passwd2 { get; set; } = string.Empty;
    }
}
