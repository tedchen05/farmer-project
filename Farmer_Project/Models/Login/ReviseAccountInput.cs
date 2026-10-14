namespace Farmer_Project.Models.Login
{
    // 會員資料修改輸入
    public class ReviseAccountInput
    {
        public string Account { get; set; } = string.Empty;
        public string NickName { get; set; } = string.Empty;
        public string Passwd { get; set; } = string.Empty;
    }
}
