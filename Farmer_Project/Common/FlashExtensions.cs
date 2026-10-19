using Microsoft.AspNetCore.Mvc;

namespace Farmer_Project.Common
{
    // 一次性 flash 訊息, 由 layout 用 SweetAlert2 顯示
    public static class FlashExtensions
    {
        public const string MessageKey = "FlashMessage";
        public const string TypeKey = "FlashType";

        // type 對應 SweetAlert2 的 icon: success / error / warning / info
        public static void Flash(this Controller controller, string? message, string type = "success")
        {
            controller.TempData[MessageKey] = message ?? string.Empty;
            controller.TempData[TypeKey] = type;
        }
    }
}
