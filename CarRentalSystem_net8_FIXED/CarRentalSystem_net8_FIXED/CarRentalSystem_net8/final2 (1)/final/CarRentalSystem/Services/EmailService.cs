using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Logging;

namespace CarRentalSystem.Services
{
    public class EmailSettings
    {
        public string SmtpHost { get; set; } = string.Empty;
        public int SmtpPort { get; set; } = 587;
        public bool EnableSsl { get; set; } = true;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FromAddress { get; set; } = string.Empty;
        public string FromName { get; set; } = "Car Rental System";
        public string DefaultRecipient { get; set; } = string.Empty;
    }

    public interface IEmailService
    {
        Task SendAsync(string to, string subject, string htmlBody);
        Task SendDefaultAsync(string subject, string htmlBody);
    }

    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(EmailSettings settings, ILogger<EmailService> logger)
        {
            _settings = settings;
            _logger = logger;
        }

        public async Task SendAsync(string to, string subject, string htmlBody)
        {
            if (string.IsNullOrWhiteSpace(_settings.SmtpHost) || string.IsNullOrWhiteSpace(_settings.FromAddress))
            {
                _logger.LogWarning("Email skipped: SMTP not configured (Host or FromAddress missing). Subject: {Subject}, To: {To}", subject, to);
                return;
            }

            using var message = new MailMessage();
            message.From = new MailAddress(_settings.FromAddress, _settings.FromName);
            message.To.Add(new MailAddress(to));
            message.Subject = subject;
            message.Body = htmlBody;
            message.IsBodyHtml = true;

            try
            {
                using var client = new SmtpClient(_settings.SmtpHost, _settings.SmtpPort)
                {
                    EnableSsl = _settings.EnableSsl,
                    Credentials = string.IsNullOrWhiteSpace(_settings.Username)
                        ? CredentialCache.DefaultNetworkCredentials
                        : new NetworkCredential(_settings.Username, _settings.Password)
                };

                await client.SendMailAsync(message);
                _logger.LogInformation("Email sent successfully. To: {To}, Subject: {Subject}", to, subject);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Email send failed. To: {To}, Subject: {Subject}", to, subject);
            }
        }

        public Task SendDefaultAsync(string subject, string htmlBody)
        {
            var to = string.IsNullOrWhiteSpace(_settings.DefaultRecipient)
                ? _settings.FromAddress
                : _settings.DefaultRecipient;
            return SendAsync(to, subject, htmlBody);
        }
    }
}


