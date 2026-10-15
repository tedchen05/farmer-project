using Farmer_Project.Filters;
using Farmer_Project.Models.Article;
using Farmer_Project.Services.Article;
using Microsoft.AspNetCore.Mvc;

namespace Farmer_Project.Controllers
{
    public class ArticleController : Controller
    {
        private readonly IArticleService _articleService;

        public ArticleController(IArticleService articleService)
        {
            _articleService = articleService;
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
        [AdminOnly]
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

        [AdminOnly]
        [HttpPost]
        public async Task<IActionResult> FarmersArtAdd(FarmersArticlesViewModel model, string action)
        {
            bool isPublish = action == "true";

            var result = await _articleService.AddAsync(model, isPublish);
            if (!result.Success)
            {
                ModelState.AddModelError(result.ErrorField!, result.ErrorMessage!);
                return View(model);
            }

            return RedirectToAction("DashBoard", "Farmer", new { show = "FarmersArticlesList" });
        }

        // 編輯文章
        [AdminOnly]
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

        [AdminOnly]
        [HttpPost]
        public async Task<IActionResult> FarmersArtEdit(int id, FarmersArticlesViewModel model, string action)
        {
            bool isPublish = action == "true";

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

            return RedirectToAction("DashBoard", "Farmer", new { farmersId = model.FarmersId, show = "FarmersArticlesList" });
        }

        // 刪除文章
        [AdminOnly]
        [HttpPost]
        public IActionResult FarmersArtDelete(int DeleteArticlesId)
        {
            var ok = _articleService.Delete(DeleteArticlesId);
            if (!ok)
            {
                return NotFound("該文章不存在！");
            }

            return RedirectToAction("DashBoard", "Farmer", new { show = "FarmersArticlesList" });
        }
    }
}
