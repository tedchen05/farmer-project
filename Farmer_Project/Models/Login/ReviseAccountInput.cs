using System.ComponentModel.DataAnnotations;

namespace Farmer_Project.Models.Login
{
    // 會員資料修改輸入
    public class ReviseAccountInput
    {
        [Display(Name = "會員帳號")]
        public string Account { get; set; } = string.Empty;

        [Required(ErrorMessage = "請輸入會員暱稱")]
        [Display(Name = "會員暱稱")]
        public string NickName { get; set; } = string.Empty;

        [Required(ErrorMessage = "請輸入會員密碼")]
        [StringLength(12, MinimumLength = 8, ErrorMessage = "密碼需要8~12位數")]
        [Display(Name = "會員密碼")]
        public string Passwd { get; set; } = string.Empty;
    }
}
