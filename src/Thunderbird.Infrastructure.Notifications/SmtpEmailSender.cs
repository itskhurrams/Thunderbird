using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using Thunderbird.Application.Interfaces;
using Thunderbird.Domain.Models;

namespace Thunderbird.Infrastructure.Notifications {
    public class SmtpEmailSender : IEmailSender {
        private readonly EmailSettings _settings;
        public SmtpEmailSender(IOptions<EmailSettings> settings) {
            _settings = settings.Value;
        }

        public async Task SendAsync(string toAddress, string subject, string body) {
            using SmtpClient client = new(_settings.SmtpHost, _settings.SmtpPort) {
                EnableSsl = _settings.EnableSsl
            };
            if (!string.IsNullOrEmpty(_settings.Username)) {
                client.Credentials = new NetworkCredential(_settings.Username, _settings.Password);
            }

            using MailMessage message = new(
                new MailAddress(_settings.FromAddress, _settings.FromName),
                new MailAddress(toAddress)) {
                Subject = subject,
                Body = body
            };

            await client.SendMailAsync(message);
        }
    }
}
