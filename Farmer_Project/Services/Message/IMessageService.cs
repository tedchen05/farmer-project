using Farmer_Project.Models.Message;

namespace Farmer_Project.Services.Message
{
    // 寄送訊息商業邏輯
    public interface IMessageService
    {
        MessageResult SendPromoToAll(string message);
    }
}
