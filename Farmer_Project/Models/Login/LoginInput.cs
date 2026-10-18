using System.ComponentModel.DataAnnotations;

namespace Farmer_Project.Models.Login
{
    // 登入輸入
    public class LoginInput
    {
        [Required(ErrorMessage = "請輸入會員帳號")]
        [EmailAddress(ErrorMessage = "帳號格式錯誤，請輸入電子信箱")]
        [Display(Name = "會員帳號")]
        public string Account { get; set; } = string.Empty;

        [Required(ErrorMessage = "請輸入會員密碼")]
        [Display(Name = "會員密碼")]
        public string Passwd { get; set; } = string.Empty;
    }
}
