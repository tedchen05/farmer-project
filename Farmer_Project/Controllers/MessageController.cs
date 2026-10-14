using Farmer_Project.Services.Message;
using Microsoft.AspNetCore.Mvc;

namespace Farmer_Project.Controllers
{
    public class MessageController : Controller
    {
        private readonly IMessageService _messageService;

        public MessageController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpPost]
        public IActionResult SendEmail(string message, string actionType)
        {
            if (actionType == "yes")
            {
                var result = _messageService.SendPromoToAll(message);
                TempData["LabelResult"] = result.Message;
            }

            TempData["messageBtn"] = true;
            return RedirectToAction("DashBoard", "Farmers");
        }
    }
}
