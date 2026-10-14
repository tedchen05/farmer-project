using MailKit.Net.Smtp;
using MimeKit;

namespace Farmer_Project.Helpers
{
    // 寄送 Email 的服務
    public class EmailHelper
    {
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _senderName;
        private readonly string _senderEmail;
        private readonly string _appPassword;

        public EmailHelper(IConfiguration configuration)
        {
            var settings = configuration.GetSection("EmailSettings");
            _smtpServer = settings["SmtpServer"];
            _smtpPort = int.Parse(settings["Port"]);
            _senderName = settings["SenderName"];
            _senderEmail = settings["SenderEmail"];
            _appPassword = settings["AppPassword"];
        }

        // 寄送一封 Email
        public void SendEmail(string email, string subject, string body)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_senderName, _senderEmail));
            message.To.Add(new MailboxAddress("", email));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder { HtmlBody = body };
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
