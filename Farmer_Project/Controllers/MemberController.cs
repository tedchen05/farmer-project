using Farmer_Project.Common;
using Farmer_Project.Entities;
using Farmer_Project.Models.Common;
using Farmer_Project.Models.Member;
using Farmer_Project.Services.Member;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Farmer_Project.Controllers
{
    [Authorize(Roles = "Admin")]
    public class MemberController : Controller
    {
        private readonly IMemberService _memberService;

        public MemberController(IMemberService memberService)
        {
            _memberService = memberService;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(string? sort, string? nickName, string? account, string? passwd)
        {
            var result = await _memberService.CreateAsync(sort, nickName, account, passwd);
            if (!result.Valid)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return View();
            }

            this.Flash("新增成功");
            return RedirectToAction("Index");
        }

        // 會員列表與搜尋(分頁)
        public async Task<IActionResult> Index(string? searchTerm, int page = 1, int pageSize = 10)
        {
            pageSize = PagedList<MemberInformation>.NormalizePageSize(pageSize);
            var members = await _memberService.SearchAsync(searchTerm, page, pageSize);

            var viewModel = new MemberListViewModel
            {
                Members = members,
                SearchTerm = searchTerm
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var member = await _memberService.GetByIdAsync(id);
            if (member == null)
            {
                return RedirectToAction("Index");
            }
            return View(member);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, string? sort, string? nickName, string? account, string? passwd)
        {
            var result = await _memberService.UpdateAsync(id, sort, nickName, account, passwd);
            if (!result.Valid)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return View("Edit", new MemberInformation
                {
                    Id = id,
                    Sort = sort ?? string.Empty,
                    NickName = nickName ?? string.Empty,
                    Account = account ?? string.Empty,
                    Passwd = passwd ?? string.Empty
                });
            }

            this.Flash("資料更新成功");
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _memberService.DeleteAsync(id);
            this.Flash("刪除成功");
            return RedirectToAction("Index");
        }
    }
}
