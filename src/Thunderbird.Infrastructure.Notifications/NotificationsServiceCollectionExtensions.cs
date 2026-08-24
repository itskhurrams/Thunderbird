using Microsoft.Extensions.DependencyInjection;
using Thunderbird.Application.Interfaces;

namespace Thunderbird.Infrastructure.Notifications {
    public static class NotificationsServiceCollectionExtensions {
        public static IServiceCollection AddNotifications(this IServiceCollection services) {
            services.AddScoped<IEmailSender, SmtpEmailSender>();
            services.AddHttpClient<IWhatsAppSender, WhatsAppCloudSender>();
            return services;
        }
    }
}
