namespace Farmer_Project.Models.Login
{
    // 註冊輸入
    public class RegisterInput
    {
        public string NickName { get; set; } = string.Empty;
        public string Account { get; set; } = string.Empty;
        public string Passwd { get; set; } = string.Empty;
    }
}
