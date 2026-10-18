using Farmer_Project.Common;
using Farmer_Project.Models.Login;
using Farmer_Project.Services.Login;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
        public async Task<IActionResult> Login(LoginInput member)
        {
            ViewData["BodyClass"] = "sub_page";

            if (!ModelState.IsValid)
            {
                return View(member);
            }

            var result = await _loginService.LoginAsync(member.Account, member.Passwd); // 帳號驗證

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

            // 建立驗證票證
            string role = result.Role == MemberRole.Admin ? UserRoles.Admin : UserRoles.Member;
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, member.Account),
                new Claim(ClaimTypes.Role, role)
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

            if (result.Role == MemberRole.Admin)
            {
                return RedirectToAction("Index", "Member");
            }

            return RedirectToAction("Index", "Home");
        }

        // 後台登出
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        // 一般會員登出
        [HttpGet]
        public async Task<IActionResult> Logout1()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
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
            string account = User.Identity?.Name ?? string.Empty;
            if (string.IsNullOrEmpty(account))
            {
                return RedirectToAction("Login");
            }

            ViewData["BodyClass"] = "sub_page";
            return View(new ReviseAccountInput { Account = account });
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterInput input)
        {
            ViewData["BodyClass"] = "sub_page";

            if (!ModelState.IsValid)
            {
                return View(input);
            }

            var result = await _loginService.RegisterAsync(input);
            if (result.Success)
            {
                TempData["message"] = result.Message;
                return RedirectToAction("Login");
            }

            ApplyError(result);
            return View(input);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPasswd(ForgetPasswordInput input)
        {
            ViewData["BodyClass"] = "sub_page";

            if (!ModelState.IsValid)
            {
                return View(input);
            }

            var result = await _loginService.CheckAccountAsync(input.Account);
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
        public async Task<IActionResult> NewPasswd(NewPasswordInput input)
        {
            ViewData["BodyClass"] = "sub_page";

            if (!ModelState.IsValid)
            {
                return View(input);
            }

            string account = HttpContext.Session.GetString(SessionKeys.ResetPwdUserId) ?? string.Empty;
            var result = await _loginService.ChangePasswordAsync(account, input.Passwd, input.Passwd2);
            if (result.Success)
            {
                TempData["message"] = result.Message;
                return RedirectToAction("Login");
            }

            ApplyError(result);
            return View(input);
        }

        [HttpPost]
        public async Task<IActionResult> ReviseAccount(ReviseAccountInput input)
        {
            string account = User.Identity?.Name ?? string.Empty;
            if (string.IsNullOrEmpty(account))
            {
                return RedirectToAction("Login");
            }

            ViewData["BodyClass"] = "sub_page";
            if (!ModelState.IsValid)
            {
                input.Account = account;
                return View(input);
            }

            var result = await _loginService.ReviseAccountAsync(account, input.NickName, input.Passwd);
            TempData["message"] = result.Message;
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
