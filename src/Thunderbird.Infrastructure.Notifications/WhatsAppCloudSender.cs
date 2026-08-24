using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Thunderbird.Application.Interfaces;
using Thunderbird.Domain.Models;

namespace Thunderbird.Infrastructure.Notifications {
    // Sends OTP codes via Meta's WhatsApp Cloud API (https://graph.facebook.com).
    // Requires an approved message template - see WhatsAppSettings.TemplateName.
    public class WhatsAppCloudSender : IWhatsAppSender {
        private readonly HttpClient _httpClient;
        private readonly WhatsAppSettings _settings;

        public WhatsAppCloudSender(HttpClient httpClient, IOptions<WhatsAppSettings> settings) {
            _httpClient = httpClient;
            _settings = settings.Value;

            _httpClient.BaseAddress = new Uri("https://graph.facebook.com/v20.0/");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _settings.AccessToken);
        }

        public async Task SendVerificationCodeAsync(string toPhoneNumber, string code) {
            var payload = new {
                messaging_product = "whatsapp",
                to = ToWhatsAppPhoneFormat(toPhoneNumber),
                type = "template",
                template = new {
                    name = _settings.TemplateName,
                    language = new { code = _settings.TemplateLanguageCode },
                    components = new object[] {
                        new {
                            type = "body",
                            parameters = new object[] {
                                new { type = "text", text = code }
                            }
                        }
                    }
                }
            };

            using StringContent content = new(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            using var response = await _httpClient.PostAsync($"{_settings.PhoneNumberId}/messages", content);
            response.EnsureSuccessStatusCode();
        }

        // Meta's API expects the recipient number without a leading '+' (our stored numbers are
        // validated as E.164, e.g. "+12025550123").
        private static string ToWhatsAppPhoneFormat(string phoneNumber) => phoneNumber.TrimStart('+');
    }
}
