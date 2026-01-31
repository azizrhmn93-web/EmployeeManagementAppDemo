using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;

namespace EmployeeManagement.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration config;

        public EmailSender(IConfiguration config)
        {
            this.config = config;
        }

        public async Task SendEmailAsync(string email, string subject, string body)
        {
            var mail = config["EmailSettings:Mail"];
            var pw = config["EmailSettings:Password"];
            var host = config["EmailSettings:Host"];
            var port = int.Parse(config["EmailSettings:Port"] ?? "587");

            var client = new SmtpClient(host, port)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(mail, pw)
            };

            var fromAddress = new MailAddress("aziz.rhmn93@gmail.com", "Team Manager");
            var toAddress = new MailAddress(email);
            var Message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true 
            };

            await client.SendMailAsync(Message);

        }
    }
}
