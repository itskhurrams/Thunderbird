namespace Thunderbird.Application.Interfaces {
    public interface IEmailSender {
        Task SendAsync(string toAddress, string subject, string body);
    }
}
