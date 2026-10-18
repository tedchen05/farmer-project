using Farmer_Project.Filters;
using Farmer_Project.Services.Message;
using Microsoft.AspNetCore.Mvc;

namespace Farmer_Project.Controllers
{
    [AdminOnly]
    public class MessageController : Controller
    {
        private readonly IMessageService _messageService;

        public MessageController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SendEmail(string message, string actionType)
        {
            if (actionType == "yes")
            {
                var result = _messageService.SendPromoToAll(message);
                TempData["LabelResult"] = result.Message;
            }

            return RedirectToAction("Index");
        }
    }
}
