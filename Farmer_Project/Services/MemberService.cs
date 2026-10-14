using Farmer_Project.Models.Entity;

namespace Farmer_Project.Services
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

    public class MemberService : IMemberService
    {
        private readonly AppDbContext _dbContext;

        public MemberService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // 驗證帳號密碼並回傳登入結果
        public LoginResult Login(string account, string passwd)
        {
            var member = _dbContext.MemberInformations.FirstOrDefault(m => m.Account == account);

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
                Role = member.Sort == "1" ? MemberRole.Admin : MemberRole.Member
            };
        }

        // 確認帳號是否已存在
        public bool IsAccountTaken(string account)
        {
            return _dbContext.MemberInformations.Any(m => m.Account == account);
        }

        // 更新指定帳號的密碼
        public void UpdatePassword(string account, string newPasswd)
        {
            var member = _dbContext.MemberInformations.FirstOrDefault(m => m.Account == account);

            if (member != null)
            {
                member.Passwd = newPasswd;
                _dbContext.SaveChanges();
            }
        }

        // 註冊新會員，成功回傳true
        public bool Register(string nickName, string account, string passwd)
        {
            if (string.IsNullOrEmpty(nickName) || string.IsNullOrEmpty(account) || string.IsNullOrEmpty(passwd))
            {
                return false;
            }

            if (!account.Contains(".com") || passwd.Length < 8 || passwd.Length > 12)
            {
                return false;
            }

            if (IsAccountTaken(account))
            {
                return false;
            }

            var member = new MemberInformation
            {
                Sort = "0",
                NickName = nickName,
                Account = account,
                Passwd = passwd
            };

            _dbContext.MemberInformations.Add(member);
            _dbContext.SaveChanges();
            return true;
        }
    }
}
