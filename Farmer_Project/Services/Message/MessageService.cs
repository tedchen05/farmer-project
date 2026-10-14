using Farmer_Project.Helpers;
using Farmer_Project.Models.Message;
using Farmer_Project.Repositories.Member;

namespace Farmer_Project.Services.Message
{
    public class MessageService : IMessageService
    {
        private readonly IMemberRepository _memberRepository;
        private readonly EmailHelper _emailHelper;

        public MessageService(IMemberRepository memberRepository, EmailHelper emailHelper)
        {
            _memberRepository = memberRepository;
            _emailHelper = emailHelper;
        }

        // 寄送優惠碼給所有會員
        public MessageResult SendPromoToAll(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                return new MessageResult { Message = "請輸入有效的訊息內容！" };
            }

            var members = _memberRepository.GetAll();
            if (members.Count == 0)
            {
                return new MessageResult { Message = "沒有找到任何收件人地址！" };
            }

            var result = new MessageResult { Message = "信件發送成功！" };
            foreach (var member in members)
            {
                try
                {
                    var body = $@"
                    <div style='font-family: Arial, sans-serif; line-height: 1.6;'>
                        <p>{member.NickName}您好，以下是我們小農平台的優惠碼：</p>
                        <p>{message}</p>
                        <p>如您有其他需求，可以隨時透過以下方式聯絡我們：</p>
                        <ul>
                            <li>客服信箱：lccent@gmail.com</li>
                            <li>聯絡電話：(02)2364-8885</li>
                            <li>聯絡地址：100台北市中正區羅斯福路三段272號3樓</li>
                        </ul>
                        <p>祝好，<br>小農分享平台服務團隊</p>
                    </div>";

                    _emailHelper.SendEmail(member.Account, "小農平台優惠碼", body);
                }
                catch (Exception ex)
                {
                    result.Message = "信件發送失敗！錯誤信息：" + ex.Message;
                }
            }

            return result;
        }
    }
}
