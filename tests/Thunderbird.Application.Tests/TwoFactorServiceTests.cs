using Thunderbird.Application.Services;
using Thunderbird.Application.Tests.TestDoubles;
using Thunderbird.Domain.Entities;

namespace Thunderbird.Application.Tests {
    public class TwoFactorServiceTests {
        private static User CreateUser() => new() {
            UserId = 1, LoginName = "jdoe", LoginPassword = "irrelevant",
            FirstName = "John", LastName = "Doe",
            Email = "jdoe@example.com", PhoneNumber = "+12025550123"
        };

        private static (TwoFactorService Service, FakeEmailSender Email, FakeWhatsAppSender WhatsApp) CreateService() {
            var email = new FakeEmailSender();
            var whatsApp = new FakeWhatsAppSender();
            var service = new TwoFactorService(new FakeMemoryCacheProvider(), email, whatsApp);
            return (service, email, whatsApp);
        }

        [Fact]
        public async Task IssueChallenge_SendsTheSameCodeToBothEmailAndWhatsApp() {
            var (service, email, whatsApp) = CreateService();
            var user = CreateUser();

            await service.IssueChallenge(user);

            Assert.Single(email.SentMessages);
            Assert.Single(whatsApp.SentCodes);
            Assert.Equal(user.Email, email.SentMessages[0].ToAddress);
            Assert.Equal(user.PhoneNumber, whatsApp.SentCodes[0].ToPhoneNumber);

            string codeFromEmail = ExtractCode(email.SentMessages[0].Body);
            string codeFromWhatsApp = whatsApp.SentCodes[0].Code;
            Assert.Equal(codeFromEmail, codeFromWhatsApp);
            Assert.Equal(6, codeFromEmail.Length);
        }

        [Fact]
        public async Task Verify_Succeeds_WithTheCorrectCode() {
            var (service, email, _) = CreateService();
            var user = CreateUser();
            string challengeId = await service.IssueChallenge(user);
            string code = ExtractCode(email.SentMessages[0].Body);

            var result = await service.Verify(challengeId, code);

            Assert.True(result.Succeeded);
            Assert.Equal(user.UserId, result.User!.UserId);
        }

        [Fact]
        public async Task Verify_Fails_ForUnknownChallengeId() {
            var (service, _, _) = CreateService();

            var result = await service.Verify("does-not-exist", "123456");

            Assert.False(result.Succeeded);
        }

        [Fact]
        public async Task Verify_Fails_ForWrongCode_ButChallengeStaysUsableForFurtherAttempts() {
            var (service, email, _) = CreateService();
            var user = CreateUser();
            string challengeId = await service.IssueChallenge(user);
            string correctCode = ExtractCode(email.SentMessages[0].Body);

            var wrongAttempt = await service.Verify(challengeId, "000000".Equals(correctCode) ? "111111" : "000000");
            Assert.False(wrongAttempt.Succeeded);

            var correctAttempt = await service.Verify(challengeId, correctCode);
            Assert.True(correctAttempt.Succeeded);
        }

        [Fact]
        public async Task Verify_IsSingleUse_SecondAttemptFailsEvenWithCorrectCode() {
            var (service, email, _) = CreateService();
            var user = CreateUser();
            string challengeId = await service.IssueChallenge(user);
            string code = ExtractCode(email.SentMessages[0].Body);

            var first = await service.Verify(challengeId, code);
            var second = await service.Verify(challengeId, code);

            Assert.True(first.Succeeded);
            Assert.False(second.Succeeded);
        }

        [Fact]
        public async Task Verify_LocksOutAfterFiveWrongAttempts() {
            var (service, email, _) = CreateService();
            var user = CreateUser();
            string challengeId = await service.IssueChallenge(user);
            string correctCode = ExtractCode(email.SentMessages[0].Body);
            string wrongCode = "000000".Equals(correctCode) ? "111111" : "000000";

            for (int i = 0; i < 5; i++) {
                await service.Verify(challengeId, wrongCode);
            }

            var afterLockout = await service.Verify(challengeId, correctCode);

            Assert.False(afterLockout.Succeeded);
        }

        private static string ExtractCode(string messageBody) {
            var match = System.Text.RegularExpressions.Regex.Match(messageBody, @"\d{6}");
            Assert.True(match.Success, $"No 6-digit code found in: {messageBody}");
            return match.Value;
        }
    }
}
