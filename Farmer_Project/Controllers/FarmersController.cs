using Farmer_Project.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace Farmer_Project.Controllers
{
    public class FarmersController : Controller
    {
        //注入AppDbContext(調用操作資料表的CRUD)
        private readonly AppDbContext _dbContext;


        //建構子
        public FarmersController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        //小農頁面
        [HttpGet]
        public IActionResult FarmersIndex(string CropsType)
        {
            ViewData["BodyClass"] = "sub_page";

            if (string.IsNullOrWhiteSpace(CropsType))
            {
                ViewBag.Message = "請輸入有效的作物類型！";
                return View(new List<FarmersArticles>());
            }

            try
            {
                var farmers = _dbContext.FarmersArticles
                    .Include(f => f.FarmersInfo)
                    .Include(fd => fd.FarmersArticlesDetails)
                    .Where(a => a.FarmersInfo != null &&
                                a.FarmersInfo.CropsType != null &&
                                a.FarmersInfo.CropsType.ToLower() == CropsType.ToLower() &&
                                a.ArticleType == "小農介紹" &&
                                a.IsPublished == true)
                    .ToList();

                if (!farmers.Any())
                {
                    ViewBag.Message = "沒有找到符合條件的文章！";
                }

                return View(farmers);
            }
            catch (Exception ex)
            {
                Console.WriteLine("查詢發生錯誤：" + ex.Message);
                ViewBag.Message = "發生錯誤，請稍後再試。";
                return View(new List<FarmersArticles>());
            }
        }
        /*var farmers = _dbContext.FarmersArticles
            .Include(f => f.FarmersInfo)
            .Include(fd => fd.FarmersArticlesDetails)
            .Where(a => a.FarmersInfo.CropsType.ToLower() == CropsType.ToLower() && 
                        a.ArticleType == "小農介紹" && 
                        a.IsPublished == true)
            .ToList();
            return View(farmers);*/

        //小農故事
        [HttpGet]
        public IActionResult FarmerDetail(int FarmersId, int ArticlesId)
        {
            ViewData["BodyClass"] = "sub_page";

            var article = _dbContext.FarmersArticles
                        .Include(f => f.FarmersInfo)
                        .Include(fad => fad.FarmersArticlesDetails)
                        .Where(a => a.FarmersId == FarmersId &&
                                    a.ArticlesId == ArticlesId &&
                                    a.IsPublished == true)
                        .FirstOrDefault();

            if (article == null)
            {
                return NotFound("文章尚未建置或不存在");
            }

            return View(article);
        }

        //部落格
        [HttpGet]
        public IActionResult Blog(string? CropsType)
        {
            ViewData["BodyClass"] = "sub_page";

            var articleQuery = _dbContext.FarmersArticles
                        .Include(a => a.FarmersInfo)
                        .Where(a => a.ArticleType == "一般文章" && a.IsPublished == true);

            //根據CropsType篩選文章
            if (!string.IsNullOrEmpty(CropsType) && CropsType != "All")
            {
                articleQuery = articleQuery.Where(a => a.FarmersInfo.CropsType == CropsType);
            }

            var article = articleQuery.OrderByDescending(a => a.CreatedDate).ToList();

            return View(article);
        }
        //部落格文章
        [HttpGet]
        public IActionResult Articles(int FarmersId, int ArticlesId)
        {
            ViewData["BodyClass"] = "sub_page";

            var article = _dbContext.FarmersArticles
                .Include(a => a.FarmersInfo)
                .Include(a => a.FarmersArticlesDetails)
                .Where(a => a.ArticlesId == ArticlesId &&
                            a.FarmersId == FarmersId &&
                            a.IsPublished == true)
                .FirstOrDefault();

            if (article == null)
            {
                return NotFound();
            }

            return View(article);
        }


        //後台管理頁面(主頁)
        public async Task<IActionResult> DashBoard()
        {
            var farmers = await _dbContext.FarmersInfo
                .Include(f => f.FarmersArticles)
                .ToListAsync();

            foreach (var farmer in farmers)
            {
                farmer.FarmersArticles ??= new List<FarmersArticles>();
            }

            var memberInformations = await _dbContext.MemberInformations.ToListAsync();

            // 創建 FarmersAndMembersViewModel 實例並將資料傳遞
            var viewModel = new FarmersAndMembersViewModel
            {
                FarmersInfos = farmers,
                MemberInformations = memberInformations
            };

            return View(viewModel); // 傳遞 ViewModel 到 DashBoard 視圖
        }
        //登出
        public IActionResult LogOut()
        {
            HttpContext.Session.Remove("IsLoggedIn");
            return RedirectToAction("Index", "Home");
        }

        //-- start of 小農基本資料 --//
        //搜尋
        [HttpGet]
        public IActionResult FarmersInfoSearch(string Search)
        {
            var viewModel = new FarmersAndMembersViewModel();

            if (string.IsNullOrEmpty(Search))
            {
                // 如果沒有提供搜尋字串，則返回所有資料
                var farmers = _dbContext.FarmersInfo.Include(f => f.FarmersArticles).ToList();
                return View("DashBoard", farmers);
            }

            //轉換成英文
            var transSearch = TransDictionary(Search);

            // 使用轉換後的字串進行搜尋
            viewModel.FarmersInfos = _dbContext.FarmersInfo
                                        .Where(f => f.Name.Contains(Search) ||
                                                    f.FarmName.Contains(Search) ||
                                                    f.CropsType.Contains(transSearch) ||
                                                    f.Address.Contains(Search) ||
                                                    f.Phone.Contains(Search))
                                        .Include(f => f.FarmersArticles)
                                        .ToList();

            TempData["show"] = "FarmersInfoTable";
            return View("DashBoard", viewModel);
        }

        //新增
        [HttpGet]
        public IActionResult FarmersInfoAdd()
        {
            var model = new FarmersInfo();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> FarmersInfoAdd(FarmersInfo farmersInfo, IFormFile? image)
        {
            try
            {
                if (image != null)
                {
                    // 判斷圖片格式是否有效
                    if (!IsImageFormat(image))
                    {
                        ModelState.AddModelError("image", "請上傳有效的圖片格式（jpg, jpeg, png, gif）");
                        return View(farmersInfo);
                    }

                    // 計算圖片的 MD5 哈希值
                    var md5Hash = ComputeMd5Hash(image);

                    // 儲存圖片並取得圖片路徑
                    string imagePath = await SaveImage(image, md5Hash);

                    // 設定 farmersInfo 的圖片路徑
                    farmersInfo.Image = imagePath;
                }
                else
                {
                    // 沒有上傳圖片，使用預設圖片路徑
                    farmersInfo.Image = "/images/f-1.jpg";
                }

                // 儲存資料到資料庫
                _dbContext.Add(farmersInfo);
                await _dbContext.SaveChangesAsync();

                ViewBag.Message = "資料儲存成功！";
                return View(new FarmersInfo());
            }
            catch
            {
                ViewBag.Message = "農場名或 Email已有資料，請更換！";
                return View(farmersInfo);
            }
        }

        //修改
        [HttpGet]
        public IActionResult FarmersInfoEdit(int id)
        {
            if (id == 0)
            {
                return BadRequest("無效的農場ID");
            }

            var farmer = _dbContext.FarmersInfo.FirstOrDefault(f => f.FarmersId == id);

            if (farmer == null)
            {
                return NotFound("找不到對應的農場資料");
            }

            var model = new FarmersInfo
            {
                FarmersId = farmer.FarmersId,
                Name = farmer.Name,
                FarmName = farmer.FarmName,
                Image = farmer.Image,
                Address = farmer.Address,
                Phone = farmer.Phone,
                Email = farmer.Email,
                CropsType = farmer.CropsType,
                PlantType = farmer.PlantType
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> FarmersInfoEdit(int id, FarmersInfo model, IFormFile? image)
        {
            if (id != 0)
            {
                var farmer = await _dbContext.FarmersInfo.FirstOrDefaultAsync(f => f.FarmersId == id);

                if (image != null)
                {
                    if (!IsImageFormat(image))
                    {
                        ModelState.AddModelError("image", "請上傳有效的圖片格式（jpg, jpeg, png, gif）");
                        return View(model);
                    }

                    //圖片處理
                    var md5Hash = ComputeMd5Hash(image);
                    string imagePath = await SaveImage(image, md5Hash);
                    farmer.Image = imagePath;
                }

                //更換新資料
                farmer.Name = model.Name;
                farmer.FarmName = model.FarmName;
                farmer.Address = model.Address;
                farmer.Phone = model.Phone;
                farmer.Email = model.Email;
                farmer.CropsType = model.CropsType;
                farmer.PlantType = model.PlantType;
            }

            // 保存變更
            await _dbContext.SaveChangesAsync();
            TempData["SuccessMessage"] = "小農資料修改成功！";

            //匿名對象 new { show = "FarmersInfoTable" }: 傳遞參數
            return RedirectToAction("DashBoard", new { show = "FarmersInfoTable" });
        }

        //刪除
        [HttpPost]
        public async Task<IActionResult> FarmersInfoDelete(int DeleteFarmersId)
        {
            var farmer = await _dbContext.FarmersInfo.FirstOrDefaultAsync(f => f.FarmersId == DeleteFarmersId);
            if (farmer != null)
            {
                // 刪除資料
                _dbContext.FarmersInfo.Remove(farmer);
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                return NotFound("找不到對應的農場資料");
            }

            // 設置刪除成功消息
            TempData["SuccessMessage"] = "小農資料刪除成功！";

            return RedirectToAction("DashBoard", new { show = "FarmersInfoTable" });
        }
        //-- end of 小農基本資料 --//


        //-- start of 小農文章 --//
        //文章，段落新增
        [HttpGet]
        public IActionResult FarmersArtAdd(int id)
        {
            var viewModel = new FarmersArticlesViewModel
            {
                FarmersId = id,
                Articles = new ArticleViewModel(),
                ArticleDetails = new List<FarmersArticlesDetailsViewModel>()
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> FarmersArtAdd(FarmersArticlesViewModel model, string action)
        {
            //判斷: "true":發佈 / "false":保存
            bool isPublish = action == "true";

            IFormFile image = model.Articles.ArticleImage; //前端傳來的圖片
            string imagePath = null;
            string subImagePath = null;

            //處理文章圖片
            if (image != null)
            {
                if (!IsImageFormat(image))
                {
                    ModelState.AddModelError("image", "請上傳有效的圖片格式（jpg, jpeg, png, gif）");
                    return View(model);
                }

                //圖片處理
                var md5Hash = ComputeMd5Hash(image);
                imagePath = await SaveImage(image, md5Hash);
            }
            else
            {
                imagePath = "/images/default.jpg";
            }

            //賦值給Entity
            var ArticleEntity = new FarmersArticles
            {
                FarmersId = model.FarmersId,
                ArticleType = model.Articles.ArticleType ?? "未選擇",
                ArticleTitle = model.Articles.ArticleTitle ?? "未上傳",
                ArticleImagePath = imagePath,
                ArticleSummary = model.Articles.ArticleSummary ?? "未上傳",
                IsPublished = isPublish
            };


            //資料庫保存
            _dbContext.FarmersArticles.Add(ArticleEntity);
            await _dbContext.SaveChangesAsync();

            //文章ID
            var articleID = ArticleEntity.ArticlesId;

            //處理段落
            foreach (var detail in model.ArticleDetails)
            {
                //處理段落圖片
                if (detail.SubImage != null)
                {
                    if (!IsImageFormat(detail.SubImage))
                    {
                        ModelState.AddModelError("image", "請上傳有效的圖片格式（jpg, jpeg, png, gif）");
                        return View(model);
                    }

                    //圖片處理
                    var md5Hash = ComputeMd5Hash(detail.SubImage);
                    subImagePath = await SaveImage(detail.SubImage, md5Hash);
                }
                else
                {
                    subImagePath = "/images/default.jpg";
                }

                //段落儲存
                var articleDetail = new FarmersArticlesDetails
                {
                    ArticlesId = articleID,
                    DetailId = detail.DetailId,
                    SubTitle = detail.SubTitle ?? "未上傳",
                    SubImagePath = subImagePath,
                    SubContent = detail.SubContent ?? "未上傳"
                };
                await _dbContext.FarmersArticlesDetails.AddAsync(articleDetail);
            }
            await _dbContext.SaveChangesAsync();


            return RedirectToAction("DashBoard", new { show = "FarmersArticlesList" });
        }

        //文章，段落編輯
        [HttpGet]
        public async Task<IActionResult> FarmersArtEdit(int id)
        {
            //根據ArticleId撈文章
            var article = await _dbContext.FarmersArticles
                            .Include(a => a.FarmersArticlesDetails) //撈段落資料
                            .FirstOrDefaultAsync(a => a.ArticlesId == id);

            var model = new FarmersArticlesViewModel
            {
                FarmersId = (int)article.FarmersId,
                ArticlesId = article.ArticlesId,
                Articles = new ArticleViewModel
                {
                    ArticleType = article.ArticleType,
                    ArticleTitle = article.ArticleTitle,
                    ArticleImagePath = article.ArticleImagePath,
                    ArticleSummary = article.ArticleSummary
                },

                ArticleDetails = article.FarmersArticlesDetails.Select(detail => new FarmersArticlesDetailsViewModel
                {
                    DetailId = detail.DetailId,
                    SubTitle = detail.SubTitle,
                    SubImagePath = detail.SubImagePath,
                    SubContent = detail.SubContent
                }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> FarmersArtEdit(int id, FarmersArticlesViewModel model, string action)
        {
            //判斷: "true":發佈 / "false":保存
            bool isPublish = action == "true";


            //檢查文章
            var article = await _dbContext.FarmersArticles
                .Include(a => a.FarmersArticlesDetails) //撈段落資料
                .FirstOrDefaultAsync(a => a.ArticlesId == model.ArticlesId && a.FarmersId == model.FarmersId); //判斷該文章是否該小農文章

            if (article == null)
            {
                return NotFound("文章或農民不存在，或者文章不屬於該農民。");
            }


            IFormFile image = model.Articles.ArticleImage;
            string imagePath = null;

            //處理文章圖片
            if (image != null)
            {
                if (!IsImageFormat(image))
                {
                    ModelState.AddModelError("image", "請上傳有效的圖片格式（jpg, jpeg, png, gif）");
                    return View(model);
                }
                //圖片處理
                var md5Hash = ComputeMd5Hash(image);
                imagePath = await SaveImage(image, md5Hash);
            }
            else
            {
                imagePath = article.ArticleImagePath; //使用原本圖片
            }

            //更新文章內容
            article.ArticleType = model.Articles.ArticleType ?? "未選擇";
            article.ArticleTitle = model.Articles.ArticleTitle ?? "未填寫";
            article.ArticleSummary = model.Articles.ArticleSummary ?? "未填寫";
            article.ArticleImagePath = imagePath ?? "未上傳";
            article.IsPublished = isPublish;

            // 處理段落
            foreach (var detailData in model.ArticleDetails)
            {
                // 找對應的段落
                var detail = article.FarmersArticlesDetails.FirstOrDefault(d => d.DetailId == detailData.DetailId);
                string subImagePath = null;

                // 如果段落存在，則更新它
                if (detail != null)
                {
                    if (detailData.SubImage != null)
                    {
                        if (!IsImageFormat(detailData.SubImage))
                        {
                            ModelState.AddModelError("subImage", "請上傳有效的段落圖片格式（jpg, jpeg, png, gif）");
                            return View(model);
                        }
                        var md5Hash = ComputeMd5Hash(detailData.SubImage);
                        subImagePath = await SaveImage(detailData.SubImage, md5Hash);
                    }
                    else
                    {
                        subImagePath = detail.SubImagePath;  // 如果沒有新的圖片，使用原本的圖片路徑
                    }

                    // 更新段落
                    detail.SubTitle = detailData.SubTitle ?? "未填寫";
                    detail.SubImagePath = subImagePath;
                    detail.SubContent = detailData.SubContent ?? "未填寫";
                }
                else if (detail == null)
                {
                    // 若段落不存在，則新增段落
                    var newDetail = new FarmersArticlesDetails
                    {
                        ArticlesId = (int)model.ArticlesId,
                        DetailId = detailData.DetailId,
                        SubTitle = detailData.SubTitle ?? "未填寫",
                        SubImagePath = subImagePath ?? "未上傳",
                        SubContent = detailData.SubContent ?? "未填寫"
                    };

                    if (detailData.SubImage != null)
                    {
                        if (!IsImageFormat(detailData.SubImage))
                        {
                            ModelState.AddModelError("subImage", "請上傳有效的段落圖片格式（jpg, jpeg, png, gif）");
                            return View(model);
                        }
                        var md5Hash = ComputeMd5Hash(detailData.SubImage);
                        subImagePath = await SaveImage(detailData.SubImage, md5Hash);
                        newDetail.SubImagePath = subImagePath;
                    }
                    else
                    {
                        newDetail.SubImagePath = "/images/default.jpg";
                    }

                    // 新增段落到文章中
                    article.FarmersArticlesDetails.Add(newDetail);
                }
            }

            //提交的段落
            var submittedDetailIds = model.ArticleDetails.Select(d => d.DetailId).ToList();

            //撈出的段落 排除提交的段落(多的段落-刪除)
            var detailsToRemove = article.FarmersArticlesDetails.Where(d => !submittedDetailIds.Contains(d.DetailId)).ToList();

            foreach (var item in detailsToRemove)
            {
                article.FarmersArticlesDetails.Remove(item);
            }

            // 儲存更改
            await _dbContext.SaveChangesAsync();

            return RedirectToAction("DashBoard", new { farmersId = model.FarmersId, show = "FarmersArticlesList" });
        }


        //文章刪除
        [HttpPost]
        public async Task<IActionResult> FarmersArtDelete(int DeleteArticlesId)
        {
            var article = await _dbContext.FarmersArticles.FirstOrDefaultAsync(a => a.ArticlesId == DeleteArticlesId);

            if (article == null)
            {
                return NotFound("該文章不存在！");
            }

            _dbContext.FarmersArticles.Remove(article);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction("Dashboard", new { show = "FarmersArticlesList" });
        }


        //------start of 一般方法-------------------------------------------------------------------------------------------------//       

        //判斷圖片格式
        private bool IsImageFormat(IFormFile image)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var fileExtension = Path.GetExtension(image.FileName)?.ToLower();
            return allowedExtensions.Contains(fileExtension);
        }

        //圖片內容MD-5編碼
        private string ComputeMd5Hash(IFormFile image)
        {
            //using區塊結束後會釋放MD5占用的資源
            using (var md5 = MD5.Create())
            {
                using (var stream = image.OpenReadStream())  // 讀取檔案流
                {
                    byte[] hashBytes = md5.ComputeHash(stream);
                    StringBuilder sb = new StringBuilder();
                    foreach (var b in hashBytes)
                    {
                        sb.Append(b.ToString("x2")); // 轉換為十六進位格式
                    }
                    return sb.ToString();
                }
            }
        }

        //圖片存進wwwroot
        private async Task<string> SaveImage(IFormFile image, string md5Hash)
        {
            //產生一個唯一的圖片檔案名稱，使用 MD5 作為檔名
            string fileExtension = Path.GetExtension(image.FileName); // 獲取圖片副檔名
            string fileName = md5Hash + fileExtension;  // 以 MD5 哈希值作為檔名

            //設定儲存路徑 (Directory.GetCurrentDirectory() -返回當前應用程式的根目錄路徑)
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

            // 檢查圖片是否已存在，如果已存在則不儲存並返回現有的檔案名稱
            if (System.IO.File.Exists(filePath))
            {
                return "/images/" + fileName;  // 返回現有圖片的路徑
            }

            // 儲存檔案 
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream); // 複製圖片資料到伺服器
            }
            return "/images/" + fileName; ;
        }

        // 定義中文到英文的映射
        private string TransDictionary(string input)
        {
            // 如果是英文，直接返回
            if (input.All(c => char.IsLetter(c) && char.IsAscii(c)))
            {
                return input;
            }

            var translateDict = new Dictionary<string, string>
            {
                { "咖啡", "Coffee" },
                { "芒果", "Mango" }
            };

            // 如果字典中有對應的中文，則返回對應的英文，否則返回原始的中文
            if (translateDict.TryGetValue(input, out var translated))
            {
                return translated;
            }

            return input;
        }


        //------end of 一般方法-------------------------------------------------------------------------------------------------//
    }


}
