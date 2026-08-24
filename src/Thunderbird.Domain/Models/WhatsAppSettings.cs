using System.ComponentModel.DataAnnotations;

namespace Thunderbird.Domain.Models {
    public class WhatsAppSettings {
        [Required(AllowEmptyStrings = false)]
        public required string AccessToken { get; set; }
        [Required(AllowEmptyStrings = false)]
        public required string PhoneNumberId { get; set; }
        // Must already be an approved WhatsApp message template - free-form text cannot be
        // sent for a business-initiated conversation like a login verification code.
        public string TemplateName { get; set; } = "otp_verification";
        public string TemplateLanguageCode { get; set; } = "en_US";
    }
}
