using MailKit.Net.Smtp;
using MimeKit;

namespace To_Do_List.Service
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public void SendEmail(string to, string subject, string body)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("ToDo List App", _configuration["EmailSettings:FromEmail"]));
            message.To.Add(new MailboxAddress("", to));
            message.Subject = subject;

            message.Body = new TextPart("plain")
            {
                Text = body
            };

            using (var client = new SmtpClient())
            {
                client.Connect(_configuration["EmailSettings:SmtpServer"], int.Parse(_configuration["EmailSettings:SmtpPort"]), false);
                client.Authenticate(_configuration["EmailSettings:FromEmail"], _configuration["EmailSettings:Password"]);

                client.Send(message);
                client.Disconnect(true);
            }
        }
    }
}
