using Farmer_Project.Common;
using Microsoft.AspNetCore.Authorization;
using Farmer_Project.Models.Article;
using Farmer_Project.Services.Article;
using Farmer_Project.Services.Farmer;
using Microsoft.AspNetCore.Mvc;

namespace Farmer_Project.Controllers
{
    public class ArticleController : Controller
    {
        private readonly IArticleService _articleService;
        private readonly IFarmerService _farmerService;

        public ArticleController(IArticleService articleService, IFarmerService farmerService)
        {
            _articleService = articleService;
            _farmerService = farmerService;
        }

        // 後台文章列表
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Index()
        {
            var viewModel = _farmerService.GetDashboard();
            return View(viewModel);
        }

        // 部落格
        [HttpGet]
        public IActionResult Blog(string? CropsType)
        {
            ViewData["BodyClass"] = "sub_page";
            var articles = _articleService.GetBlog(CropsType);
            return View(articles);
        }

        // 部落格文章
        [HttpGet]
        public IActionResult Articles(int FarmersId, int ArticlesId)
        {
            ViewData["BodyClass"] = "sub_page";
            var article = _articleService.GetArticle(FarmersId, ArticlesId);
            if (article == null)
            {
                return NotFound();
            }

            return View(article);
        }

        // 新增文章
        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> FarmersArtAdd(FarmersArticlesViewModel model, string action)
        {
            bool isPublish = action == ArticleAction.Publish;

            var result = await _articleService.AddAsync(model, isPublish);
            if (!result.Success)
            {
                ModelState.AddModelError(result.ErrorField!, result.ErrorMessage!);
                return View(model);
            }

            return RedirectToAction("Index");
        }

        // 編輯文章
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult FarmersArtEdit(int id)
        {
            var viewModel = _articleService.BuildEditModel(id);
            if (viewModel == null)
            {
                return NotFound("文章不存在");
            }

            return View(viewModel);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> FarmersArtEdit(int id, FarmersArticlesViewModel model, string action)
        {
            bool isPublish = action == ArticleAction.Publish;

            var result = await _articleService.EditAsync(model, isPublish);
            if (result.NotFound)
            {
                return NotFound("文章或農民不存在，或者文章不屬於該農民。");
            }
            if (!result.Success)
            {
                ModelState.AddModelError(result.ErrorField!, result.ErrorMessage!);
                return View(model);
            }

            return RedirectToAction("Index");
        }

        // 刪除文章
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult FarmersArtDelete(int DeleteArticlesId)
        {
            var ok = _articleService.Delete(DeleteArticlesId);
            if (!ok)
            {
                return NotFound("該文章不存在！");
            }

            return RedirectToAction("Index");
        }
    }
}
