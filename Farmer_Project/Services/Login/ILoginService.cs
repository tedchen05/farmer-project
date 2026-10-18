using Farmer_Project.Models.Login;

namespace Farmer_Project.Services.Login
{
    // 登入相關商業邏輯
    public interface ILoginService
    {
        Task<LoginResult> LoginAsync(string account, string passwd);
        Task<OperationResult> RegisterAsync(RegisterInput input);
        Task<OperationResult> CheckAccountAsync(string account);
        Task<OperationResult> ChangePasswordAsync(string account, string passwd, string passwd2);
        Task<OperationResult> ReviseAccountAsync(string account, string nickName, string passwd);
    }
}
