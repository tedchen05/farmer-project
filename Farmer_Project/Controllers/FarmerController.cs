using Farmer_Project.Common;
using Farmer_Project.Entities;
using Farmer_Project.Models.Common;
using Farmer_Project.Models.Farmer;
using Microsoft.AspNetCore.Authorization;
using Farmer_Project.Services.Farmer;
using Microsoft.AspNetCore.Mvc;

namespace Farmer_Project.Controllers
{
    public class FarmerController : Controller
    {
        private readonly IFarmerService _farmerService;

        public FarmerController(IFarmerService farmerService)
        {
            _farmerService = farmerService;
        }

        // 小農列表
        [HttpGet]
        public async Task<IActionResult> FarmersIndex(string CropsType)
        {
            ViewData["BodyClass"] = "sub_page";

            if (string.IsNullOrWhiteSpace(CropsType))
            {
                this.Flash("請輸入有效的作物類型！", "info");
                return View(new List<FarmersArticles>());
            }

            var farmers = await _farmerService.GetFarmerIntrosAsync(CropsType);
            if (!farmers.Any())
            {
                this.Flash("沒有找到符合條件的文章！", "info");
            }

            return View(farmers);
        }

        // 小農故事
        [HttpGet]
        public async Task<IActionResult> FarmerDetail(int FarmersId, int ArticlesId)
        {
            ViewData["BodyClass"] = "sub_page";

            var article = await _farmerService.GetFarmerStoryAsync(FarmersId, ArticlesId);
            if (article == null)
            {
                return NotFound("文章尚未建置或不存在");
            }

            return View(article);
        }

        // 後台小農列表與搜尋(分頁)
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Index(string? searchTerm, int page = 1, int pageSize = 10)
        {
            pageSize = PagedList<FarmersInfo>.NormalizePageSize(pageSize);
            var farmers = await _farmerService.GetFarmerListAsync(searchTerm, page, pageSize);

            var viewModel = new FarmerListViewModel
            {
                Farmers = farmers,
                SearchTerm = searchTerm
            };

            return View(viewModel);
        }

        // 新增小農
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult FarmersInfoAdd()
        {
            return View(new FarmersInfo());
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> FarmersInfoAdd(FarmersInfo farmersInfo, IFormFile? image)
        {
            var result = await _farmerService.AddAsync(farmersInfo, image);

            if (result.ImageInvalid)
            {
                ModelState.AddModelError("image", result.Message);
                return View(farmersInfo);
            }

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return View(farmersInfo);
            }

            this.Flash("新增成功");
            return RedirectToAction("Index");
        }

        // 修改小農
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> FarmersInfoEdit(int id)
        {
            if (id == 0)
            {
                return BadRequest("無效的農場ID");
            }

            var farmer = await _farmerService.GetByIdAsync(id);
            if (farmer == null)
            {
                return NotFound("找不到對應的農場資料");
            }

            return View(farmer);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> FarmersInfoEdit(int id, FarmersInfo model, IFormFile? image)
        {
            var result = await _farmerService.EditAsync(id, model, image);

            if (result.ImageInvalid)
            {
                ModelState.AddModelError("image", result.Message);
                return View(model);
            }

            this.Flash("修改成功");
            return RedirectToAction("Index");
        }

        // 刪除小農
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> FarmersInfoDelete(int DeleteFarmersId)
        {
            var ok = await _farmerService.DeleteAsync(DeleteFarmersId);
            if (!ok)
            {
                return NotFound("找不到對應的農場資料");
            }

            this.Flash("刪除成功");
            return RedirectToAction("Index");
        }
    }
}
