using System.Security.Cryptography;
using Thunderbird.Application.Interfaces;
using Thunderbird.Application.Models;
using Thunderbird.Domain.Entities;
using Thunderbird.Domain.Interfaces;

namespace Thunderbird.Application.Services {
    public class TwoFactorService : ITwoFactorService {
        private const int CodeLength = 6;
        private const int MaxAttempts = 5;
        private static readonly TimeSpan ChallengeLifetime = TimeSpan.FromMinutes(5);

        private readonly IMemoryCacheProvider _memoryCacheProvider;
        private readonly IEmailSender _emailSender;
        private readonly IWhatsAppSender _whatsAppSender;

        public TwoFactorService(IMemoryCacheProvider memoryCacheProvider, IEmailSender emailSender, IWhatsAppSender whatsAppSender) {
            _memoryCacheProvider = memoryCacheProvider;
            _emailSender = emailSender;
            _whatsAppSender = whatsAppSender;
        }

        public async Task<string> IssueChallenge(User user) {
            string code = NumericCodeGenerator.Generate(CodeLength);
            string challengeId = Convert.ToHexString(RandomNumberGenerator.GetBytes(24));

            var challenge = new TwoFactorChallenge {
                User = user,
                Code = code,
                ExpiresAt = DateTimeOffset.UtcNow.Add(ChallengeLifetime),
                AttemptsRemaining = MaxAttempts
            };
            _memoryCacheProvider.SetCache(ChallengeKey(challengeId), challenge, challenge.ExpiresAt);

            string emailBody = $"Your Thunderbird verification code is {code}. It expires in 5 minutes.";
            await Task.WhenAll(
                _emailSender.SendAsync(user.Email, "Your verification code", emailBody),
                _whatsAppSender.SendVerificationCodeAsync(user.PhoneNumber, code));

            return challengeId;
        }

        public Task<TwoFactorVerificationResult> Verify(string challengeId, string code) {
            string key = ChallengeKey(challengeId);
            var challenge = _memoryCacheProvider.GetFromCache<TwoFactorChallenge>(key);
            if (challenge is null) {
                return Task.FromResult(TwoFactorVerificationResult.Failed("The code has expired or is invalid. Please log in again."));
            }

            if (challenge.Code != code) {
                challenge.AttemptsRemaining--;
                if (challenge.AttemptsRemaining <= 0) {
                    _memoryCacheProvider.ClearCache(key);
                    return Task.FromResult(TwoFactorVerificationResult.Failed("Too many incorrect attempts. Please log in again."));
                }

                // Re-set with the original absolute expiry preserved, so failed attempts
                // cannot be used to keep extending the challenge's lifetime.
                _memoryCacheProvider.SetCache(key, challenge, challenge.ExpiresAt);
                return Task.FromResult(TwoFactorVerificationResult.Failed("Incorrect code."));
            }

            _memoryCacheProvider.ClearCache(key);
            return Task.FromResult(TwoFactorVerificationResult.Success(challenge.User));
        }

        private static string ChallengeKey(string challengeId) => $"2fa:{challengeId}";
    }
}
