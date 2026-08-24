using System.ComponentModel.DataAnnotations;

namespace Thunderbird.Domain.Models {
    public class TokenAuthenticationSettings {
        [Required(AllowEmptyStrings = false)]
        public required string SecretKey { get; set; }
        [Required(AllowEmptyStrings = false)]
        public required string Issuer { get; set; }
        [Required(AllowEmptyStrings = false)]
        public required string Audience { get; set; }
        [Required(AllowEmptyStrings = false)]
        public required string CookieName { get; set; }
        [Range(1, int.MaxValue)]
        public int ExpiryMinutes { get; set; } = 60;
    }
}
