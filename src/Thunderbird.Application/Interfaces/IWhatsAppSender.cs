namespace Thunderbird.Application.Interfaces {
    public interface IWhatsAppSender {
        // WhatsApp business-initiated messages must use a pre-approved template, so this is
        // deliberately not a free-text send like IEmailSender - only a code substitution.
        Task SendVerificationCodeAsync(string toPhoneNumber, string code);
    }
}
