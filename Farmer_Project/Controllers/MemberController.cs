using Farmer_Project.Entities;
using Microsoft.AspNetCore.Authorization;
using Farmer_Project.Services.Member;
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
        public async Task<IActionResult> Create(string sort, string nickName, string account, string passwd)
        {
            var result = await _memberService.CreateAsync(sort, nickName, account, passwd);
            if (!result.Valid)
            {
                TempData["dele"] = true;
                ViewBag.Message = result.Message;
                return View();
            }

            TempData["Message"] = result.Message;
            return RedirectToAction("Index");
        }

        // 會員列表與搜尋
        public async Task<IActionResult> Index(string searchTerm)
        {
            var members = await _memberService.SearchAsync(searchTerm);

            if (members.Count == 0)
            {
                ViewBag.Message = "未找到對應的會員。";
            }

            var viewModel = new FarmersAndMembersViewModel
            {
                MemberInformations = members,
                SearchTerm = searchTerm
            };

            ViewBag.SearchTerm = searchTerm;
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
        public async Task<IActionResult> Update(int id, string sort, string nickName, string account, string passwd)
        {
            var result = await _memberService.UpdateAsync(id, sort, nickName, account, passwd);
            TempData["Message"] = result.Message;
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _memberService.DeleteAsync(id);
            return RedirectToAction("Index");
        }
    }
}
