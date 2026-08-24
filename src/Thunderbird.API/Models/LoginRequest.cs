using System.ComponentModel.DataAnnotations;

namespace Thunderbird.API.Models {
    public record LoginRequest(
        [Required, StringLength(256, MinimumLength = 1)] string LoginName,
        [Required, StringLength(256, MinimumLength = 1)] string LoginPassword,
        [Required] long CaptchaId,
        [Required, StringLength(4, MinimumLength = 4)] string CaptchaCode);
}
