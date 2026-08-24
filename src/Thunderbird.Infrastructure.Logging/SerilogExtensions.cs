using Microsoft.Extensions.Hosting;
using Serilog;

namespace Thunderbird.Infrastructure.Logging {
    public static class SerilogExtensions {
        public static IHostBuilder UseThunderbirdLogging(this IHostBuilder hostBuilder) {
            return hostBuilder.UseSerilog((context, services, configuration) => configuration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext());
        }
    }
}
