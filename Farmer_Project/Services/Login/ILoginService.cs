using Farmer_Project.Models.Login;

namespace Farmer_Project.Services.Login
{
    // 登入相關商業邏輯
    public interface ILoginService
    {
        LoginResult Login(string account, string passwd);
        OperationResult Register(RegisterInput input);
        OperationResult CheckAccount(string account);
        OperationResult ChangePassword(string account, string passwd, string passwd2);
        OperationResult ReviseAccount(string account, string nickName, string passwd);
    }
}
