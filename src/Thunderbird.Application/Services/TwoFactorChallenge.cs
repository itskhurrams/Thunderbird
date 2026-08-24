using Thunderbird.Domain.Entities;

namespace Thunderbird.Application.Services {
    internal class TwoFactorChallenge {
        public required User User { get; init; }
        public required string Code { get; init; }
        public required DateTimeOffset ExpiresAt { get; init; }
        public int AttemptsRemaining { get; set; }
    }
}
