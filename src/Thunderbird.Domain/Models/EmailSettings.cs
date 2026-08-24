using System.ComponentModel.DataAnnotations;

namespace Thunderbird.Domain.Models {
    public class EmailSettings {
        [Required(AllowEmptyStrings = false)]
        public required string SmtpHost { get; set; }
        [Range(1, 65535)]
        public int SmtpPort { get; set; } = 587;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        [Required(AllowEmptyStrings = false)]
        public required string FromAddress { get; set; }
        public string FromName { get; set; } = "Thunderbird";
        public bool EnableSsl { get; set; } = true;
    }
}
