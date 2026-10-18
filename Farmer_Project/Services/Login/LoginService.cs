using Farmer_Project.Common;
using Farmer_Project.Entities;
using Farmer_Project.Models.Login;
using Farmer_Project.Repositories.Login;

namespace Farmer_Project.Services.Login
{
    public class LoginService : ILoginService
    {
        private readonly ILoginRepository _loginRepository;

        public LoginService(ILoginRepository loginRepository)
        {
            _loginRepository = loginRepository;
        }

        // 驗證帳號密碼並回傳登入結果
        public LoginResult Login(string account, string passwd)
        {
            var member = _loginRepository.GetByAccount(account);

            if (member == null)
            {
                return new LoginResult { Status = LoginStatus.AccountNotFound };
            }

            if (member.Passwd != passwd)
            {
                return new LoginResult { Status = LoginStatus.WrongPassword };
            }

            return new LoginResult
            {
                Status = LoginStatus.Success,
                Role = member.Sort == MemberSort.Admin ? MemberRole.Admin : MemberRole.Member
            };
        }

        // 註冊新會員
        public OperationResult Register(RegisterInput input)
        {
            if (string.IsNullOrEmpty(input.NickName) || string.IsNullOrEmpty(input.Account) || string.IsNullOrEmpty(input.Passwd))
            {
                return OperationResult.Fail(MemberField.None, "請填寫所有欄位。");
            }

            if (input.Passwd.Length < 8 || input.Passwd.Length > 12)
            {
                return OperationResult.Fail(MemberField.Passwd, "密碼需要8~12位數的符號");
            }

            if (_loginRepository.ExistsByAccount(input.Account))
            {
                return OperationResult.Fail(MemberField.Account, "帳號已經存在");
            }

            var member = new MemberInformation
            {
                Sort = "0",
                NickName = input.NickName,
                Account = input.Account,
                Passwd = input.Passwd
            };

            _loginRepository.Add(member);
            _loginRepository.Save();
            return OperationResult.Ok("註冊成功");
        }

        // 忘記密碼:確認帳號是否存在
        public OperationResult CheckAccount(string account)
        {
            if (string.IsNullOrEmpty(account))
            {
                return OperationResult.Fail(MemberField.None, "請填寫所有欄位。");
            }

            if (!_loginRepository.ExistsByAccount(account))
            {
                return OperationResult.Fail(MemberField.Account, "這個帳號不存在");
            }

            return OperationResult.Ok("帳號確認成功");
        }

        // 忘記密碼:驗證後更新密碼
        public OperationResult ChangePassword(string account, string passwd, string passwd2)
        {
            if (string.IsNullOrEmpty(passwd) || string.IsNullOrEmpty(passwd2))
            {
                return OperationResult.Fail(MemberField.None, "請填寫所有欄位。");
            }

            if (passwd.Length < 8 || passwd.Length > 12 || passwd2.Length < 8 || passwd2.Length > 12 || passwd != passwd2)
            {
                return OperationResult.Fail(MemberField.Passwd, "密碼輸入錯誤");
            }

            var member = _loginRepository.GetByAccount(account);
            if (member == null)
            {
                return OperationResult.Fail(MemberField.Account, "找不到該帳號的資料");
            }

            member.Passwd = passwd;
            _loginRepository.Save();
            return OperationResult.Ok("密碼更改完成");
        }

        // 會員資料修改
        public OperationResult ReviseAccount(string account, string nickName, string passwd)
        {
            if (string.IsNullOrEmpty(account))
            {
                return OperationResult.Fail(MemberField.None, "無效的帳號或尚未登入");
            }

            var member = _loginRepository.GetByAccount(account);
            if (member == null)
            {
                return OperationResult.Fail(MemberField.None, "找不到該帳號的資料");
            }

            member.NickName = nickName;
            member.Passwd = passwd;
            _loginRepository.Save();
            return OperationResult.Ok("資料更新成功");
        }
    }
}
