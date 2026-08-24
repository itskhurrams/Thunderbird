using Thunderbird.Domain.Entities;

namespace Thunderbird.Application.Models {
    public record TwoFactorVerificationResult(bool Succeeded, string? Error, User? User) {
        public static TwoFactorVerificationResult Success(User user) => new(true, null, user);
        public static TwoFactorVerificationResult Failed(string error) => new(false, error, null);
    }
}
