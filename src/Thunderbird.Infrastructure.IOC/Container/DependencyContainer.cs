using Microsoft.Extensions.DependencyInjection;

using Thunderbird.Application.Interfaces;
using Thunderbird.Application.Services;
using Thunderbird.Domain.Interfaces;
using Thunderbird.Infrastructure.Caching;
using Thunderbird.Infrastructure.Persistance.Repositories;

namespace Thunderbird.Infrastructure.IOC.Container {

    public class DependencyContainer {
        public static void RegisterServices(IServiceCollection services) {

            ServicesRegistration(services);
            RepositoryRegistration(services);
        }
        private static void ServicesRegistration(IServiceCollection services) {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITerritoryService, TerritoryService>();
        }
        private static void RepositoryRegistration(IServiceCollection services) {
            services.AddScoped<IBaseRepository, BaseRepository>();

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ITerritoryRepository, TerritoryRepository>();
            services.AddScoped<IMemoryCacheProvider, MemoryCacheProvider>();
        }
    }
}
