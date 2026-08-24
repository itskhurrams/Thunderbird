using Thunderbird.Application.Interfaces;

namespace Thunderbird.Application.Tests.TestDoubles {
    public class FakeEmailSender : IEmailSender {
        public List<(string ToAddress, string Subject, string Body)> SentMessages { get; } = new();

        public Task SendAsync(string toAddress, string subject, string body) {
            SentMessages.Add((toAddress, subject, body));
            return Task.CompletedTask;
        }
    }
}
