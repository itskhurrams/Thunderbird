using System.ComponentModel.DataAnnotations;

namespace Thunderbird.API.Models {
    public record TwoFactorVerifyRequest(
        [Required] string ChallengeId,
        [Required, StringLength(6, MinimumLength = 6)] string Code);
}
