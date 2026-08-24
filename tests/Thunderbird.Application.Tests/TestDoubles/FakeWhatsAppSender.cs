using Thunderbird.Application.Interfaces;

namespace Thunderbird.Application.Tests.TestDoubles {
    public class FakeWhatsAppSender : IWhatsAppSender {
        public List<(string ToPhoneNumber, string Code)> SentCodes { get; } = new();

        public Task SendVerificationCodeAsync(string toPhoneNumber, string code) {
            SentCodes.Add((toPhoneNumber, code));
            return Task.CompletedTask;
        }
    }
}
