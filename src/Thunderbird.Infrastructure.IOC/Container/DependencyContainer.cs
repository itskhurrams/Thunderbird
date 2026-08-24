using Microsoft.Extensions.DependencyInjection;

using Thunderbird.Application.Interfaces;
using Thunderbird.Application.Services;
using Thunderbird.Domain.Interfaces;
using Thunderbird.Infrastructure.Caching;
using Thunderbird.Infrastructure.Notifications;
using Thunderbird.Infrastructure.Persistance.Repositories;

namespace Thunderbird.Infrastructure.IOC.Container {

    public class DependencyContainer {
        public static void RegisterServices(IServiceCollection services) {

            ServicesRegistration(services);
            RepositoryRegistration(services);
            services.AddNotifications();
        }
        private static void ServicesRegistration(IServiceCollection services) {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITerritoryService, TerritoryService>();
            services.AddScoped<ICaptchaService, CaptchaService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<ITwoFactorService, TwoFactorService>();
            services.AddSingleton<IPasswordHasher, PasswordHasher>();
        }
        private static void RepositoryRegistration(IServiceCollection services) {
            services.AddScoped<IBaseRepository, BaseRepository>();

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ITerritoryRepository, TerritoryRepository>();
            services.AddScoped<ICaptchaRepository, CaptchaRepository>();
            services.AddScoped<IMemoryCacheProvider, MemoryCacheProvider>();
        }
    }
}
