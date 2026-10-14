using Farmer_Project.Models.ViewModel;
using Farmer_Project.Services;
using Microsoft.AspNetCore.Mvc;


namespace Farmer_Project.Controllers
{

    public class LoginController : Controller
    {
        private readonly AppDbContext _DbContext;
        private readonly IMemberService _memberService;


        public LoginController(AppDbContext DbContext, IMemberService memberService)
        {
            _DbContext = DbContext;
            _memberService = memberService;
        }

        // 註冊登入［忘記密碼］
        public IActionResult resetPasswd()
        {
            ViewData["BodyClass"] = "sub_page";   // 此用於頁面上出現個空白框
            return View();
        }

        public IActionResult reviseAccount()
        {
            // 返回修改會員資料的視圖
            ViewData["BodyClass"] = "sub_page";
            return View();
        }

        [HttpPost]
        public ActionResult reviseAccount(AccountViewModel model)
        {
            if (ModelState.IsValid)
            {
                // 從 Cookie 中取得帳號資料
                string userAccount = HttpContext.Request.Cookies["UserAccount"];

                // 如果帳號不存在於 Cookie 中，顯示錯誤訊息
                if (string.IsNullOrEmpty(userAccount))
                {
                    ViewBag.Msg = "無效的帳號或尚未登入";
                    return View();
                }

                // 根據帳號從資料庫查詢會員資料
                var user = _DbContext.MemberInformations.SingleOrDefault(u => u.Account == userAccount);

                // 如果找不到對應的會員資料，顯示錯誤訊息
                if (user == null)
                {
                    ViewBag.Msg = "找不到該帳號的資料";
                    return View();
                }

                // 若找到會員資料，則更新資料
                user.NickName = model.Nickname; // 更新會員暱稱
                user.Passwd = model.Passwd;     // 更新會員密碼（注意密碼應進行加密處理）

                // 儲存變更
                _DbContext.SaveChanges();
                ViewBag.Msg = "資料更新成功";
            }
            else
            {
                ViewBag.Msg = "資料無效，請再試一次";
            }
            return RedirectToAction("index", "Home");
        }

        [HttpPost]
        public IActionResult resetPasswd(forgetPasswd userAccount)
        {
            ViewData["BodyClass"] = "sub_page";   // 此用於頁面上出現個空白框
            bool accountExists = _memberService.IsAccountTaken(userAccount.account);   // 確認帳號是否存在
            // 驗證表單是否完整
            if (string.IsNullOrEmpty(userAccount.account))
            {
                TempData["dele"] = true;
                ViewBag.Message = "請填寫所有欄位。";
                return View();
            }
            else if (!userAccount.account.Contains(".com"))
            {
                ViewBag.Msg1 = "帳號輸入錯誤";
                return View();
            }
            else if (!accountExists)
            {
                ViewBag.Msg1 = "這個帳號不存在";
                return View();
            }
            TempData["message"] = "帳號確認成功";

            // 以下用於將帳號儲存於Session，以便後面使用
            string account = userAccount.account;
            HttpContext.Session.SetString("ResetPwdUserId", account);  // 此表示如：Session[ResetPwdUserId] = account;
            // 結束
            return RedirectToAction("newPasswd");    // 注意點選的按鈕是否在<form></form>之間
        }


        public IActionResult newPasswd()
        {
            ViewData["BodyClass"] = "sub_page";   // 此用於頁面上出現個空白框
            return View();
        }

        [HttpPost]
        public IActionResult newPasswd(forgetPasswd userPasswd)
        {
            ViewData["BodyClass"] = "sub_page";   // 此用於頁面上出現個空白框
            string account = HttpContext.Session.GetString("ResetPwdUserId");  // 此用於將儲存的變數 "Session["ResetPwdUserId"]"代入
            try
            {
                _memberService.UpdatePassword(account, userPasswd.passwd);   // 更新該帳號密碼
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            };
            // 驗證表單是否完整
            if (string.IsNullOrEmpty(userPasswd.passwd) || string.IsNullOrEmpty(userPasswd.passwd2))
            {
                TempData["dele"] = true;
                ViewBag.Message = "請填寫所有欄位。";
                return View();
            }
            else if (userPasswd.passwd.Length < 8 || userPasswd.passwd.Length > 12 || userPasswd.passwd2.Length < 8 || userPasswd.passwd2.Length > 12 || userPasswd.passwd != userPasswd.passwd2)
            {
                ViewBag.Msg1 = "密碼輸入錯誤";
                return View();
            }
            TempData["message"] = "密碼更改完成";
            return RedirectToAction("login", "Home");  // 注意點選的按鈕是否在<form></form>之間
        }
        // 結束

        // 註冊登入［註冊會員］
        public IActionResult register()
        {
            ViewData["BodyClass"] = "sub_page";   // 此用於頁面上出現個空白框
            return View();
        }


        // 下方的register(register user)，表示將前面的register()回傳的內容，由register.cs的變數"user"接收，然後再做其中的細分
        // 注意："register.cshtml" 的輸入格name 與 "register.cs" 的 public變數，兩者對應的名稱要一致(不分大小寫)
        [HttpPost]  // 以下的部分要上報到其他控制器，必須加上這個，否則會掛掉
        public IActionResult register(register user)
        {
            ViewData["BodyClass"] = "sub_page";   // 此用於頁面上出現個空白框
            bool accountTaken = _memberService.IsAccountTaken(user.account);   // 確認帳號是否重複
            try
            {
                _memberService.Register(user.nickName, user.account, user.passwd);   // 註冊新會員
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            // 驗證表單是否完整
            if (string.IsNullOrEmpty(user.nickName) || string.IsNullOrEmpty(user.account) || string.IsNullOrEmpty(user.passwd))
            {
                TempData["dele"] = true;
                ViewBag.Message = "請填寫所有欄位。";
                return View();
            }
            else if (user.account == null && user.passwd == null)
            {
                ViewBag.Msg2 = "帳號不可空白";
                ViewBag.Msg3 = "密碼不可空白";
                return View();
            }
            else if (user.nickName == null && user.passwd == null)
            {
                ViewBag.Msg1 = "暱稱不可空白";
                ViewBag.Msg3 = "密碼不可空白";
                return View();
            }
            else if (user.nickName == null && user.account == null)
            {
                ViewBag.Msg1 = "暱稱不可空白";
                ViewBag.Msg2 = "帳號不可空白";
                return View();
            }
            else if (user.nickName == null)
            {
                ViewBag.Msg1 = "暱稱不可空白";
                return View();
            }
            else if (user.account == null)
            {
                ViewBag.Msg2 = "帳號不可空白";
                return View();
            }
            else if (user.passwd == null)
            {
                ViewBag.Msg3 = "密碼不可空白";
                return View();
            }


            if (!user.account.Contains(".com"))
            {
                ViewBag.Msg2 = "帳號請輸入完整的電子信箱帳號";
                return View();
            }
            else if (user.passwd.Length < 8 || user.passwd.Length > 12)
            {
                ViewBag.Msg3 = "密碼需要8~12位數的符號";
                return View();
            }
            else if (accountTaken)
            {
                ViewBag.Msg2 = "帳號已經存在";
                return View();
            }
            else
            {
                TempData["message"] = "註冊成功";
            }
            return RedirectToAction("login", "Home");  // 此設定為導回 "index" 的網頁
        }
        // 結束
    }
}
