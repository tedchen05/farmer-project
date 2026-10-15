using Farmer_Project.Common;
using Farmer_Project.Models.Login;
using Farmer_Project.Services.Login;
using Microsoft.AspNetCore.Mvc;

namespace Farmer_Project.Controllers
{
    public class LoginController : Controller
    {
        private readonly ILoginService _loginService;

        public LoginController(ILoginService loginService)
        {
            _loginService = loginService;
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
                return RedirectToAction("login", "Home");
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
                return RedirectToAction("login", "Home");
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
