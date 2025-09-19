using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace CarRentalSystem.Services
{
    public class SmtpEmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public SmtpEmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendAsync(string to, string subject, string htmlBody)
        {
            var section = _config.GetSection("Email");
            var host = section["SmtpHost"];
            var port = int.Parse(section["SmtpPort"] ?? "587");
            var enableSsl = bool.Parse(section["EnableSsl"] ?? "true");
            var user = section["Username"];
            var pass = section["Password"];
            var from = section["FromAddress"] ?? user;
            var fromName = section["FromName"] ?? "Car Rental System";

            using var client = new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(user, pass),
                EnableSsl = enableSsl
            };

            var mail = new MailMessage
            {
                From = new MailAddress(from, fromName),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true
            };
            mail.To.Add(to);

            await client.SendMailAsync(mail);
        }

        public Task SendDefaultAsync(string subject, string htmlBody)
        {
            var to = _config.GetSection("Email")["DefaultRecipient"] ?? _config.GetSection("Email")["Username"];
            return SendAsync(to!, subject, htmlBody);
        }
    }
}