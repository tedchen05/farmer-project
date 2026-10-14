namespace Farmer_Project.Models.Login
{
    // 登入結果狀態
    public enum LoginStatus
    {
        AccountNotFound,
        WrongPassword,
        Success
    }

    // 會員角色
    public enum MemberRole
    {
        Member,
        Admin
    }

    // 登入結果
    public class LoginResult
    {
        public LoginStatus Status { get; set; }
        public MemberRole Role { get; set; }
    }
}
