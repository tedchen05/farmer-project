using Farmer_Project.Models.Contact;
using Farmer_Project.Services.Contact;
using Microsoft.AspNetCore.Mvc;

namespace Farmer_Project.Controllers
{
    public class HomeController : Controller
    {
        private readonly IContactService _contactService;

        public HomeController(IContactService contactService)
        {
            _contactService = contactService;
        }

        public IActionResult index()
        {
            TempData.Clear();
            return View();
        }

        public IActionResult Contact()
        {
            ViewData["BodyClass"] = "sub_page";
            return View();
        }

        public IActionResult about()
        {
            ViewData["BodyClass"] = "sub_page";
            return View();
        }

        public IActionResult Crop()
        {
            ViewData["BodyClass"] = "sub_page";
            return View();
        }

        public IActionResult testimonial()
        {
            ViewData["BodyClass"] = "sub_page";
            return View();
        }

        [HttpPost]
        public IActionResult Contact(ContactMessage model)
        {
            ViewData["BodyClass"] = "sub_page";

            if (ModelState.IsValid)
            {
                bool sent = _contactService.SendAcknowledgement(model);
                ViewBag.Message = sent ? "您的訊息已成功發送！" : "發送郵件失敗，請稍後再試。";
            }

            return View();
        }
    }
}
