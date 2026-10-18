using System.ComponentModel.DataAnnotations;

namespace Farmer_Project.Models.Login
{
    // 忘記密碼:確認帳號輸入
    public class ForgetPasswordInput
    {
        [Required(ErrorMessage = "請輸入會員帳號")]
        [EmailAddress(ErrorMessage = "帳號格式錯誤，請輸入電子信箱")]
        [Display(Name = "會員帳號")]
        public string Account { get; set; } = string.Empty;
    }
}
