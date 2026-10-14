namespace Farmer_Project.Services
{
    // 會員資料存取服務
    public interface IMemberService
    {
        LoginResult Login(string account, string passwd);
        bool IsAccountTaken(string account);
        void UpdatePassword(string account, string newPasswd);
        bool Register(string nickName, string account, string passwd);
    }
}
