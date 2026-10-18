using Microsoft.AspNetCore.Authorization;
using Farmer_Project.Services.Message;
using Microsoft.AspNetCore.Mvc;

namespace Farmer_Project.Controllers
{
    [Authorize(Roles = "Admin")]
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
        public async Task<IActionResult> SendEmail(string message, string actionType)
        {
            if (actionType == "yes")
            {
                var result = await _messageService.SendPromoToAllAsync(message);
                TempData["LabelResult"] = result.Message;
            }

            return RedirectToAction("Index");
        }
    }
}
