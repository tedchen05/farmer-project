using Farmer_Project.Helpers;
using Farmer_Project.Models;
using Farmer_Project.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace Farmer_Project.Controllers
{
    public class HomeController : Controller
    {
        private readonly EmailHelper _emailHelper;
        private readonly AppDbContext _DbContext;

        public HomeController(AppDbContext DbContext)
        {
            _emailHelper = new EmailHelper();
            _DbContext = DbContext;
        }

        public IActionResult index()
        {
            TempData.Clear();
            return View();
        }

        public IActionResult Contact()
        {
            ViewData["BodyClass"] = "sub_page";
            return View();
        }
        public IActionResult about()
        {
            ViewData["BodyClass"] = "sub_page";
            return View();
        }

        public IActionResult Crop(string keyword)
        {
            ViewData["BodyClass"] = "sub_page";
            ViewData["keyword"] = keyword;

            var Data = new List<Crop>
            {
             new Crop { Name = "Tea", Image = "~/images/f-7.jpg" },
             new Crop { Name = "Coffee", Image = "~/images/f-8.jpg" },
             new Crop { Name = "Milk", Image = "~/images/f-9.jpg" },
             new Crop { Name = "Apple", Image = "~/images/f-4.jpg" },
             new Crop { Name = "Mango", Image = "~/images/f-5.jpg" }
            };

            if (string.IsNullOrEmpty(keyword))
            {
                return View(Data);
            }

            var filteredData = Data
                .Where(c => c.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                .ToList();

            return View(filteredData);
        }

        public IActionResult testimonial()
        {
            ViewData["BodyClass"] = "sub_page";
            return View();
        }

        // 註冊登入 [login] 頁面
        public IActionResult Login()
        {
            ViewData["BodyClass"] = "sub_page";   // 此用於頁面上出現個空白框
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginModel member)
        {
            ViewData["BodyClass"] = "sub_page";

            // 驗證表單是否完整
            if (string.IsNullOrEmpty(member.account) || string.IsNullOrEmpty(member.passwd))
            {
                TempData["dele"] = true;
                ViewBag.Message = "請填寫所有欄位。";
                return View();
            }
            Response.Cookies.Append("UserAccount", member.account, new CookieOptions { Expires = DateTimeOffset.Now.AddHours(1) });

            DBmanager dbmanager = new DBmanager();
            List<string> accounts = dbmanager.loginAccounts(member, _DbContext); // 呼叫 loginAccounts 進行帳號驗證

            // 將 accounts 列表存儲到 session 中


            // 檢查帳號格式
            if (!member.account.Contains(".com"))
            {
                ViewBag.Msg2 = "帳號格式輸入錯誤";
                return View();
            }

            // 檢查帳號是否存在
            if (accounts.Contains("帳號不存在"))
            {
                ViewBag.Msg3 = "這個帳號不存在";
                return View();
            }

            // 檢查密碼錯誤
            if (accounts.Contains("密碼錯誤"))
            {
                ViewBag.Msg4 = "密碼輸入錯誤";
                return View();
            }

            // 檢查帳號是否同時存在於一般會員和管理者
            if (accounts.Contains("一般會員") && accounts.Contains("管理者"))
            {
                TempData["message"] = $"登入失敗！\n警告：{member.account}這個帳號同時擁有一般會員和管理者的權限。";
                return RedirectToAction("Login");
            }

            HttpContext.Session.SetString("IsLoggedIn", "true");
            HttpContext.Session.SetString("UserName", member.account);
            if (accounts.Contains("一般會員"))
            {
                HttpContext.Session.SetString("UserRole", "Member"); // 記錄用戶角色
                // TempData["message"] = "登入成功\n您的身分為：一般會員";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                HttpContext.Session.SetString("UserRole", "Admin"); // 記錄管理者角色
                // TempData["message"] = "登入成功\n您的身分為：系統管理者";
                return RedirectToAction("DashBoard", "Farmers");
            }
        }

        // 結束

        [HttpPost]
        public IActionResult Logout() // 後臺登出
        {
            HttpContext.Session.Clear(); // 清除所有 Session 資料
            return RedirectToAction("Index", "Home"); // 返回首頁
        }
        [HttpGet]
        public IActionResult Logout1() // 一般會員登出
        {
            HttpContext.Session.Clear(); // 清除所有 Session 資料
            return RedirectToAction("Index", "Home"); // 返回首頁
        }

        [HttpPost]
        public IActionResult Contact(ContactMessage model)
        {
            ViewData["BodyClass"] = "sub_page";

            if (ModelState.IsValid)
            {
                var subject = "感謝您的聯絡！";
                var body =  $@"
                <div style='font-family: Arial, sans-serif; line-height: 1.6;'>
                    <p>{model.Name}您好，以下是我們收到的資訊：</p>
                    <p style='font-size: 18px; font-weight: bold;'>This is a test message.</p>
                    <p>我們將於 1-3 個工作天內回覆您。如您有其他需求，可以隨時透過以下方式聯絡我們：</p>
                    <ul>
                        <li>客服信箱：lccent@gmail.com</li>
                        <li>聯絡電話：(02)2364-8885</li>
                        <li>聯絡地址：100台北市中正區羅斯福路三段272號3樓</li>
                    </ul>
                    <p>祝好，<br>小農分享平台服務團隊</p>
                </div>";

                try
                {
                    _emailHelper.SendEmail(model.Email, subject, body);
                    ViewBag.Message = "您的訊息已成功發送！";
                }
                catch
                {
                    ViewBag.Message = "發送郵件失敗，請稍後再試。";
                }
            }

            return View();
        }
    }
}
