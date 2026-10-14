using MailKit.Net.Smtp;
using MimeKit;
namespace Farmer_Project.Helpers
{
    public class EmailHelper
    {
        private readonly string _smtpServer = "smtp.gmail.com";
        private readonly int _smtpPort = 587;
        private readonly string _senderEmail = "willy103320005@gmail.com";
        private readonly string _appPassword = "bqfp uwkk culz daxe";

        public void SendEmail(string email, string subject, string body)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("小農分享平台", _senderEmail));
            message.To.Add(new MailboxAddress("", email));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder { HtmlBody = $"<p>{body}</p>" };
            message.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                client.Connect(_smtpServer, _smtpPort, false);
                client.Authenticate(_senderEmail, _appPassword);
                client.Send(message);
                client.Disconnect(true);
            }

        }
    }
}
