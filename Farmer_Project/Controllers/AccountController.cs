using Farmer_Project.Common;
using Farmer_Project.Models.Login;
using Farmer_Project.Services.Login;
using Microsoft.AspNetCore.Mvc;

namespace Farmer_Project.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILoginService _loginService;

        public AccountController(ILoginService loginService)
        {
            _loginService = loginService;
        }

        // 登入頁
        public IActionResult Login()
        {
            ViewData["BodyClass"] = "sub_page";
            return View(new LoginInput());
        }

        [HttpPost]
        public IActionResult Login(LoginInput member)
        {
            ViewData["BodyClass"] = "sub_page";

            if (!ModelState.IsValid)
            {
                return View(member);
            }

            Response.Cookies.Append("UserAccount", member.Account, new CookieOptions { Expires = DateTimeOffset.Now.AddHours(1) });

            var result = _loginService.Login(member.Account, member.Passwd); // 帳號驗證

            // 檢查帳號是否存在
            if (result.Status == LoginStatus.AccountNotFound)
            {
                ModelState.AddModelError(nameof(member.Account), "這個帳號不存在");
                return View(member);
            }

            // 檢查密碼錯誤
            if (result.Status == LoginStatus.WrongPassword)
            {
                ModelState.AddModelError(nameof(member.Passwd), "密碼輸入錯誤");
                return View(member);
            }

            HttpContext.Session.SetString(SessionKeys.IsLoggedIn, "true");
            HttpContext.Session.SetString(SessionKeys.UserName, member.Account);
            if (result.Role == MemberRole.Member)
            {
                HttpContext.Session.SetString(SessionKeys.UserRole, UserRoles.Member); // 記錄用戶角色
                return RedirectToAction("Index", "Home");
            }
            else
            {
                HttpContext.Session.SetString(SessionKeys.UserRole, UserRoles.Admin); // 記錄管理者角色
                return RedirectToAction("Index", "Member");
            }
        }

        // 後台登出
        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        // 一般會員登出
        [HttpGet]
        public IActionResult Logout1()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        // 忘記密碼頁
        public IActionResult ResetPasswd()
        {
            ViewData["BodyClass"] = "sub_page";
            return View(new ForgetPasswordInput());
        }

        // 重設密碼頁
        public IActionResult NewPasswd()
        {
            ViewData["BodyClass"] = "sub_page";
            return View(new NewPasswordInput());
        }

        // 註冊頁
        public IActionResult Register()
        {
            ViewData["BodyClass"] = "sub_page";
            return View(new RegisterInput());
        }

        // 會員資料修改頁
        public IActionResult ReviseAccount()
        {
            ViewData["BodyClass"] = "sub_page";
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterInput input)
        {
            ViewData["BodyClass"] = "sub_page";

            if (!ModelState.IsValid)
            {
                return View(input);
            }

            var result = _loginService.Register(input);
            if (result.Success)
            {
                TempData["message"] = result.Message;
                return RedirectToAction("Login");
            }

            ApplyError(result);
            return View(input);
        }

        [HttpPost]
        public IActionResult ResetPasswd(ForgetPasswordInput input)
        {
            ViewData["BodyClass"] = "sub_page";

            if (!ModelState.IsValid)
            {
                return View(input);
            }

            var result = _loginService.CheckAccount(input.Account);
            if (result.Success)
            {
                HttpContext.Session.SetString(SessionKeys.ResetPwdUserId, input.Account);
                TempData["message"] = result.Message;
                return RedirectToAction("NewPasswd");
            }

            ApplyError(result);
            return View(input);
        }

        [HttpPost]
        public IActionResult NewPasswd(NewPasswordInput input)
        {
            ViewData["BodyClass"] = "sub_page";

            if (!ModelState.IsValid)
            {
                return View(input);
            }

            string account = HttpContext.Session.GetString(SessionKeys.ResetPwdUserId) ?? string.Empty;
            var result = _loginService.ChangePassword(account, input.Passwd, input.Passwd2);
            if (result.Success)
            {
                TempData["message"] = result.Message;
                return RedirectToAction("Login");
            }

            ApplyError(result);
            return View(input);
        }

        [HttpPost]
        public IActionResult ReviseAccount(ReviseAccountInput input)
        {
            string account = HttpContext.Request.Cookies["UserAccount"] ?? string.Empty;
            _loginService.ReviseAccount(account, input.NickName, input.Passwd);
            return RedirectToAction("Index", "Home");
        }

        // 將服務回傳的錯誤對應到對應欄位的 ModelState
        private void ApplyError(OperationResult result)
        {
            string message = result.Message ?? string.Empty;
            switch (result.Field)
            {
                case MemberField.NickName:
                    ModelState.AddModelError("NickName", message);
                    break;
                case MemberField.Account:
                    ModelState.AddModelError("Account", message);
                    break;
                case MemberField.Passwd:
                    ModelState.AddModelError("Passwd", message);
                    break;
                default:
                    ModelState.AddModelError(string.Empty, message);
                    break;
            }
        }
    }
}
