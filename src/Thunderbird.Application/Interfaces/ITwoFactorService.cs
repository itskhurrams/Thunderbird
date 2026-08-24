using Thunderbird.Application.Models;
using Thunderbird.Domain.Entities;

namespace Thunderbird.Application.Interfaces {
    public interface ITwoFactorService {
        // Generates a one-time code, sends it to the user's email and phone, and returns an
        // opaque challenge id the caller must present (with the code) to Verify.
        Task<string> IssueChallenge(User user);
        Task<TwoFactorVerificationResult> Verify(string challengeId, string code);
    }
}
