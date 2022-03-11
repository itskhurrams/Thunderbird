using Microsoft.Extensions.DependencyInjection;

using Thunderbird.Application.Interfaces;
using Thunderbird.Application.Services;
using Thunderbird.Infrastructure.Persistance;
using Thunderbird.Domain.Interfaces;
using Thunderbird.Infrastructure.Caching;

namespace Thunderbird.Infrastructure.IOC.Container {

    public class DependencyContainer {
        public static void RegisterServices(IServiceCollection services) {

            //Application Services
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITerritoryService, TerritoryService>();

            //Data
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ITerritoryRepository, TerritoryRepository>();
            services.AddScoped<IMemoryCacheProvider, MemoryCacheProvider>();

        }
    }
}
