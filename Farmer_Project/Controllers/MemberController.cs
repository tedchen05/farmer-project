using Farmer_Project.Entities;
using Farmer_Project.Filters;
using Farmer_Project.Services.Member;
using Microsoft.AspNetCore.Mvc;

namespace Farmer_Project.Controllers
{
    [AdminOnly]
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
        public IActionResult Create(string sort, string nickName, string account, string passwd)
        {
            var result = _memberService.Create(sort, nickName, account, passwd);
            if (!result.Valid)
            {
                TempData["dele"] = true;
                ViewBag.Message = result.Message;
                return View();
            }

            TempData["Message"] = result.Message;
            TempData["dele"] = true;
            return RedirectToAction("DashBoard", "Farmer");
        }

        // 會員搜尋
        public IActionResult Index(string searchTerm)
        {
            var members = _memberService.Search(searchTerm);

            if (members.Count == 0)
            {
                ViewBag.Message = "未找到對應的會員。";
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                TempData["dele"] = true;
            }

            var viewModel = new FarmersAndMembersViewModel
            {
                MemberInformations = members,
                SearchTerm = searchTerm
            };

            ViewBag.SearchTerm = searchTerm;
            return View("~/Views/Farmer/DashBoard.cshtml", viewModel);
        }

        [HttpPost]
        public IActionResult ReloadData()
        {
            TempData["dele"] = true;
            return RedirectToAction("DashBoard", "Farmer");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var member = _memberService.GetById(id);
            if (member == null)
            {
                return RedirectToAction("DashBoard", "Farmer", new { show = "memberTable" });
            }
            return View(member);
        }

        [HttpPost]
        public IActionResult Update(int id, string sort, string nickName, string account, string passwd)
        {
            var result = _memberService.Update(id, sort, nickName, account, passwd);
            TempData["Message"] = result.Message;
            TempData["dele"] = true;
            return RedirectToAction("DashBoard", "Farmer");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            _memberService.Delete(id);
            TempData["dele"] = true;
            return RedirectToAction("DashBoard", "Farmer", new { show = "memberTable" });
        }
    }
}
