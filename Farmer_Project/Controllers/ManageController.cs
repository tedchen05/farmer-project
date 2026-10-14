using Farmer_Project.Helpers;
using Farmer_Project.Entities;
using Microsoft.AspNetCore.Mvc;



namespace Farmer_Project.Controllers
{
    public class ManageController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _dbContext;
        private readonly EmailHelper _emailHelper;


        public ManageController(ILogger<HomeController> logger, AppDbContext dbContext, EmailHelper emailHelper)
        {
            _logger = logger;
            _dbContext = dbContext;
            _emailHelper = emailHelper;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ReloadData()
        {
            // 重新抓取所有會員資料
            var allMembers = _dbContext.MemberInformations.ToList();
            var allFarmers = _dbContext.FarmersInfo.ToList();
            // 創建 ViewModel
            var viewModel = new FarmersAndMembersViewModel
            {
                MemberInformations = allMembers,
                FarmersInfos = allFarmers,  
                SearchTerm = string.Empty // 清空搜尋條件
            };

            TempData["dele"] = true;

            // 返回到 DashBoard 視圖並傳遞 ViewModel
            return View("~/Views/Farmers/DashBoard.cshtml", viewModel);
        }

        public IActionResult Index(string searchTerm)
        {
            // 查詢所有 memberInformation 資料
            var allMembers = _dbContext.MemberInformations.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                // 根據帳號或名稱來搜尋
                allMembers = allMembers.Where(m => m.NickName.Contains(searchTerm) || m.Account.Contains(searchTerm) || m.Sort.Contains(searchTerm));
                TempData["dele"] = true; // 搜尋時標記為顯示資料
            }

            // 將查詢結果轉換為列表
            var result = allMembers.ToList();

            // 若沒有數據，顯示找不到會員訊息
            if (!result.Any())
            {
                ViewBag.Message = "未找到對應的會員。";
            }

            // 封裝到 ViewModel
            var viewModel = new FarmersAndMembersViewModel
            {
                MemberInformations = result, // 搜尋結果
                SearchTerm = searchTerm      // 搜尋關鍵字
            };

            // 傳遞 ViewModel 給視圖
            ViewBag.SearchTerm = searchTerm;  // 傳遞搜尋關鍵字給視圖
            return View("~/Views/Farmers/DashBoard.cshtml", viewModel);
        }


        [HttpPost]
        public IActionResult Create(string sort, string nickName, string account, string passwd)
        {
            // 驗證表單是否完整
            if (string.IsNullOrEmpty(sort) || string.IsNullOrEmpty(nickName) ||
                string.IsNullOrEmpty(account) || string.IsNullOrEmpty(passwd))
            {
                TempData["dele"] = true;
                ViewBag.Message = "請填寫所有欄位。";
                return View();
            }

            // 建立 MemberInformation 實體
            var memberInformation = new MemberInformation
            {
                Sort = sort,
                NickName = nickName,
                Account = account,
                Passwd = passwd, // 存儲雜湊後的密碼
            };

            try
            {
                // 新增到資料庫
                _dbContext.MemberInformations.Add(memberInformation);
                int savedChanges = _dbContext.SaveChanges();
                TempData["Message"] = savedChanges > 0 ? "新增成功！" : "新增失敗：沒有任何改動。";
            }
            catch (Exception ex)
            {
                TempData["Message"] = "新增失敗：" + ex.Message;
            }

            TempData["dele"] = true;
            return RedirectToAction("DashBoard", "Farmers");
        }

        // 第二個 POST 方法處理發送郵件
        [HttpPost]
        public IActionResult SendEmail(string message, string actionType)
        {
            if (actionType == "yes")
            {
                if (!string.IsNullOrEmpty(message))
                {
                    TempData["messageBtn"] = true;

                    var accounts = _dbContext.MemberInformations.Select(m => new { m.Account, m.NickName }).ToList();

                    if (!accounts.Any())
                    {
                        ViewBag.LabelResult = "沒有找到任何收件人地址！";
                    }

                    foreach (var account in accounts)
                    {
                        try
                        {
                            var body = $@"
                            <div style='font-family: Arial, sans-serif; line-height: 1.6;'>
                                <p>{account.NickName}您好，以下是我們小農平台的優惠碼：</p>
                                <p>{message}</p>
                                <p>如您有其他需求，可以隨時透過以下方式聯絡我們：</p>
                                <ul>
                                    <li>客服信箱：lccent@gmail.com</li>
                                    <li>聯絡電話：(02)2364-8885</li>
                                    <li>聯絡地址：100台北市中正區羅斯福路三段272號3樓</li>
                                </ul>
                                <p>祝好，<br>小農分享平台服務團隊</p>
                            </div>";

                            _emailHelper.SendEmail(account.Account, "小農平台優惠碼", body);
                            TempData["LabelResult"] = "信件發送成功！";
                        }
                        catch (Exception innerEx)
                        {
                            TempData["LabelResult"] = "信件發送失敗！錯誤信息：" + innerEx.Message;
                        }
                    }
                }
                else
                {
                    ViewBag.LabelResult = "請輸入有效的訊息內容！";
                }
            }
            TempData["messageBtn"] = true;
            return RedirectToAction("DashBoard", "Farmers");
        }

        // 編輯方法
        public IActionResult Edit(int id)
        {
            var member = _dbContext.MemberInformations.FirstOrDefault(m => m.Id == id);
            if (member == null)
            {
                ViewBag.Message = "找不到該會員資料";
                return RedirectToAction("DashBoard", "Farmers", new { show = "memberTable" });
            }
            return View(member);  // 返回編輯頁面並傳遞會員資料
        }

        [HttpPost]
        public IActionResult Update(int id, string sort, string nickName, string account, string passwd)
        {
            var member = _dbContext.MemberInformations.FirstOrDefault(m => m.Id == id);
            if (member == null)
            {
                TempData["dele"] = true;
                ViewBag.Message = "找不到該會員資料";
                return RedirectToAction("DashBoard", "Farmers");
            }

            // 更新資料
            member.Sort = sort; // 注意：這裡的 Sort 要確保大小寫正確
            member.NickName = nickName; // 同樣確認大小寫
            member.Account = account;
            member.Passwd = passwd;

            try
            {
                // 儲存更改
                _dbContext.SaveChanges();
                TempData["Message"] = "資料更新成功！";
            }
            catch (Exception ex)
            {
                TempData["Message"] = "更新失敗：" + ex.Message;
            }
            TempData["dele"] = true;
            return RedirectToAction("DashBoard", "Farmers");
        }
        // 刪除方法
        [HttpPost]
        public ActionResult Delete(int id)
        {
            // 查找對應的 memberInformation
            var member = _dbContext.MemberInformations.Find(id);
            if (member != null)
            {
                TempData["dele"] = true;
                // 假設檔案名是基於 member.Id 或 member.nickName
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", $"{member.NickName}.jpg");  // 根據 nickName 推斷檔案名，假設為 .jpg

                if (System.IO.File.Exists(filePath))
                {
                    // 刪除檔案
                    System.IO.File.Delete(filePath);
                }

                // 刪除資料庫紀錄
                _dbContext.MemberInformations.Remove(member);

                // 儲存更改
                _dbContext.SaveChanges();
            }

            // 查詢所有小農資料
            var allFarmers = _dbContext.FarmersInfo.ToList();
            // 查詢所有會員資料
            var allMembers = _dbContext.MemberInformations.ToList();

            // 建立視圖模型
            var viewModel = new FarmersAndMembersViewModel
            {
                FarmersInfos = allFarmers,  // 傳遞小農資料
                MemberInformations = allMembers,  // 傳遞會員資料
                SearchTerm = string.Empty    // 這裡是搜尋條件
            };

            // 傳遞視圖模型到 DashBoard 視圖
            return View("~/Views/Farmers/DashBoard.cshtml", viewModel);
        }
    }
}
