using Farmer_Project.Models.Contact;
using Farmer_Project.Models.Login;
using Farmer_Project.Services.Contact;
using Farmer_Project.Services.Login;
using Microsoft.AspNetCore.Mvc;

namespace Farmer_Project.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILoginService _loginService;
        private readonly IContactService _contactService;

        public HomeController(ILoginService loginService, IContactService contactService)
        {
            _loginService = loginService;
            _contactService = contactService;
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

        public IActionResult Crop()
        {
            ViewData["BodyClass"] = "sub_page";
            return View();
        }

        public IActionResult testimonial()
        {
            ViewData["BodyClass"] = "sub_page";
            return View();
        }

        // 註冊登入 [login] 頁面
        public IActionResult Login()
        {
            ViewData["BodyClass"] = "sub_page";
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginInput member)
        {
            ViewData["BodyClass"] = "sub_page";

            // 驗證表單是否完整
            if (string.IsNullOrEmpty(member.Account) || string.IsNullOrEmpty(member.Passwd))
            {
                TempData["dele"] = true;
                ViewBag.Message = "請填寫所有欄位。";
                return View();
            }
            Response.Cookies.Append("UserAccount", member.Account, new CookieOptions { Expires = DateTimeOffset.Now.AddHours(1) });

            var result = _loginService.Login(member.Account, member.Passwd); // 帳號驗證

            // 檢查帳號格式
            if (!member.Account.Contains(".com"))
            {
                ViewBag.Msg2 = "帳號格式輸入錯誤";
                return View();
            }

            // 檢查帳號是否存在
            if (result.Status == LoginStatus.AccountNotFound)
            {
                ViewBag.Msg3 = "這個帳號不存在";
                return View();
            }

            // 檢查密碼錯誤
            if (result.Status == LoginStatus.WrongPassword)
            {
                ViewBag.Msg4 = "密碼輸入錯誤";
                return View();
            }

            HttpContext.Session.SetString("IsLoggedIn", "true");
            HttpContext.Session.SetString("UserName", member.Account);
            if (result.Role == MemberRole.Member)
            {
                HttpContext.Session.SetString("UserRole", "Member"); // 記錄用戶角色
                return RedirectToAction("Index", "Home");
            }
            else
            {
                HttpContext.Session.SetString("UserRole", "Admin"); // 記錄管理者角色
                return RedirectToAction("DashBoard", "Farmer");
            }
        }

        [HttpPost]
        public IActionResult Logout() // 後臺登出
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Logout1() // 一般會員登出
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult Contact(ContactMessage model)
        {
            ViewData["BodyClass"] = "sub_page";

            if (ModelState.IsValid)
            {
                bool sent = _contactService.SendAcknowledgement(model);
                ViewBag.Message = sent ? "您的訊息已成功發送！" : "發送郵件失敗，請稍後再試。";
            }

            return View();
        }
    }
}
