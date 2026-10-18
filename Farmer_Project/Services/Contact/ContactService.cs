using Farmer_Project.Helpers;
using Farmer_Project.Models.Contact;
using Microsoft.Extensions.Logging;

namespace Farmer_Project.Services.Contact
{
    public class ContactService : IContactService
    {
        private readonly EmailHelper _emailHelper;
        private readonly ILogger<ContactService> _logger;

        public ContactService(EmailHelper emailHelper, ILogger<ContactService> logger)
        {
            _emailHelper = emailHelper;
            _logger = logger;
        }

        // 寄送聯絡回覆信，成功回傳true
        public bool SendAcknowledgement(ContactMessage model)
        {
            var subject = "感謝您的聯絡！";
            var body = $@"
                <div style='font-family: Arial, sans-serif; line-height: 1.6;'>
                    <p>{model.Name}您好，以下是我們收到的資訊：</p>
                    <p style='font-size: 18px; font-weight: bold;'>This is a test message.</p>
                    <p>我們將於 1-3 個工作天內回覆您。如您有其他需求，可以隨時透過以下方式聯絡我們：</p>
                    <ul>
                        <li>客服信箱：lccent@gmail.com</li>
                        <li>聯絡電話：(02)2364-8885</li>
                        <li>聯絡地址：100台北市中正區羅斯福路三段272號3樓</li>
                    </ul>
                    <p>祝好，<br>小農分享平台服務團隊</p>
                </div>";

            try
            {
                _emailHelper.SendEmail(model.Email, subject, body);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "寄送聯絡回覆信失敗, 收件者: {Email}", model.Email);
                return false;
            }
        }
    }
}
