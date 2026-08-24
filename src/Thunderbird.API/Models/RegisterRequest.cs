using System.ComponentModel.DataAnnotations;

namespace Thunderbird.API.Models {
    public record RegisterRequest(
        [Required, StringLength(256, MinimumLength = 1)] string LoginName,
        [Required, StringLength(256, MinimumLength = 8)] string Password,
        [Required, StringLength(256, MinimumLength = 1)] string FirstName,
        [Required, StringLength(256, MinimumLength = 1)] string LastName,
        [Required, EmailAddress, StringLength(256)] string Email,
        [Required, RegularExpression(@"^\+[1-9]\d{1,14}$", ErrorMessage = "PhoneNumber must be in E.164 format, e.g. +12025550123.")] string PhoneNumber,
        [Required] long CaptchaId,
        [Required, StringLength(4, MinimumLength = 4)] string CaptchaCode);
}
