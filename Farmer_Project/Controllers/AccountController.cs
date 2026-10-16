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
                return RedirectToAction("DashBoard", "Farmer");
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
        public IActionResult resetPasswd()
        {
            ViewData["BodyClass"] = "sub_page";
            return View();
        }

        // 重設密碼頁
        public IActionResult newPasswd()
        {
            ViewData["BodyClass"] = "sub_page";
            return View();
        }

        // 註冊頁
        public IActionResult register()
        {
            ViewData["BodyClass"] = "sub_page";
            return View();
        }

        // 會員資料修改頁
        public IActionResult reviseAccount()
        {
            ViewData["BodyClass"] = "sub_page";
            return View();
        }

        [HttpPost]
        public IActionResult register(RegisterInput input)
        {
            ViewData["BodyClass"] = "sub_page";

            var result = _loginService.Register(input);
            if (result.Success)
            {
                TempData["message"] = result.Message;
                return RedirectToAction("Login");
            }

            ApplyError(result);
            return View();
        }

        [HttpPost]
        public IActionResult resetPasswd(ForgetPasswordInput input)
        {
            ViewData["BodyClass"] = "sub_page";

            var result = _loginService.CheckAccount(input.Account);
            if (result.Success)
            {
                HttpContext.Session.SetString(SessionKeys.ResetPwdUserId, input.Account);
                TempData["message"] = result.Message;
                return RedirectToAction("newPasswd");
            }

            ApplySingleError(result);
            return View();
        }

        [HttpPost]
        public IActionResult newPasswd(ForgetPasswordInput input)
        {
            ViewData["BodyClass"] = "sub_page";

            string account = HttpContext.Session.GetString(SessionKeys.ResetPwdUserId) ?? string.Empty;
            var result = _loginService.ChangePassword(account, input.Passwd, input.Passwd2);
            if (result.Success)
            {
                TempData["message"] = result.Message;
                return RedirectToAction("Login");
            }

            ApplySingleError(result);
            return View();
        }

        [HttpPost]
        public IActionResult reviseAccount(ReviseAccountInput input)
        {
            string account = HttpContext.Request.Cookies["UserAccount"] ?? string.Empty;
            _loginService.ReviseAccount(account, input.NickName, input.Passwd);
            return RedirectToAction("index", "Home");
        }

        // 將錯誤對應到註冊頁的各欄位訊息
        private void ApplyError(OperationResult result)
        {
            switch (result.Field)
            {
                case MemberField.NickName:
                    ViewBag.Msg1 = result.Message;
                    break;
                case MemberField.Account:
                    ViewBag.Msg2 = result.Message;
                    break;
                case MemberField.Passwd:
                    ViewBag.Msg3 = result.Message;
                    break;
                default:
                    TempData["dele"] = true;
                    ViewBag.Message = result.Message;
                    break;
            }
        }

        // 忘記密碼/重設密碼頁只有單一錯誤訊息位置
        private void ApplySingleError(OperationResult result)
        {
            if (result.Field == MemberField.None)
            {
                TempData["dele"] = true;
                ViewBag.Message = result.Message;
            }
            else
            {
                ViewBag.Msg1 = result.Message;
            }
        }
    }
}
