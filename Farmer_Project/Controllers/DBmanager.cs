using Farmer_Project.Models.Entity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Farmer_Project.Models.ViewModel;

namespace Farmer_Project.Controllers
{
    public class DBmanager
    {
        private const string connStr = "Data Source=(localdb)\\MSSQLLocalDB;Database=Farmers;User Id=ted;Password=ted123456;TrustServerCertificate=True;";


            // ［會員登入］:確認帳號密碼是否存在與正確
            public List<string> loginAccounts(LoginModel member, AppDbContext _DbContext)
        {
            List<string> accounts = new List<string>();

            var Account = _DbContext.MemberInformations.Where(M => M.Account == member.account).ToList();
            if (Account.Count == 0)
            {
                accounts.Add("帳號不存在");
                return accounts;
            }

            // 直接比對明文密碼
            var posts = _DbContext.MemberInformations.Where(M => M.Account == member.account && M.Passwd == member.passwd).ToList();

            if (posts.Count != 0)
            {
                var user = posts.First();
                if (user.Sort == "0")
                {
                    accounts.Add("一般會員");
                }
                else if (user.Sort == "1")
                {
                    accounts.Add("管理者");
                }
            }
            else
            {
                accounts.Add("密碼錯誤");
            }

            return accounts;
        }

        // ［忘記密碼］:確認帳號是否存在的部分
        public List<string> checkAccounts(forgetPasswd userAccount, AppDbContext _DbContext)
        {
            List<string> accounts = new List<string>();

            var posts = _DbContext.MemberInformations.Where(M => M.Account == $"{userAccount.account}").ToList();

            if (posts.Count != 0)
            {
                accounts.Add("1");
            }
            return accounts;
        }

        // 用於延續上一個，將帳號對應的密碼更改
        public void updatePasswd(forgetPasswd userPasswd, string account, AppDbContext _DbContext)
        {
            // 直接使用新密碼，無需雜湊處理
            string NewPwd = userPasswd.passwd;

            var posts = _DbContext.MemberInformations.Where(M => M.Account == $"{account}")
                .ExecuteUpdate(s => s.SetProperty(p => p.Passwd, NewPwd));

            _DbContext.SaveChanges();
        }

        
        

        // ［註冊新會員］
        public void registerAccount(register user, int count, AppDbContext _DbContext)
        {
            if (user.nickName != null && user.account != null && user.passwd != null && user.account.Contains(".com") && user.passwd.Length >= 8 && user.passwd.Length <= 12 && count == 0)
            {
                // 直接使用明文密碼
                string NewPwd = user.passwd;

                // 新增一筆資料至指定資料庫
                var Register = new MemberInformation
                {
                    Sort = "0",
                    NickName = $"{user.nickName}",
                    Account = $"{user.account}",
                    Passwd = $"{NewPwd}"
                };

                _DbContext.MemberInformations.Add(Register);
                _DbContext.SaveChanges();
            }
        }

        // 確認帳號是否有重複的部分
        public List<string> NoSameAccounts(register user, AppDbContext _DbContext)
        {
            List<string> accounts = new List<string>();

            var posts = _DbContext.MemberInformations.Where(M => M.Account == $"{user.account}").ToList();

            if (posts.Count != 0)
            {
                accounts.Add("1");
            }
            return accounts;
        }

        internal List<string> updateAccount(object member, AppDbContext dbContext)
        {
            throw new NotImplementedException();
        }
    }
}
