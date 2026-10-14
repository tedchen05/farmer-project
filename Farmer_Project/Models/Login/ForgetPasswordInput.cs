namespace Farmer_Project.Models.Login
{
    // 忘記密碼輸入(確認帳號與重設密碼共用)
    public class ForgetPasswordInput
    {
        public string Account { get; set; } = string.Empty;
        public string Passwd { get; set; } = string.Empty;
        public string Passwd2 { get; set; } = string.Empty;
    }
}
