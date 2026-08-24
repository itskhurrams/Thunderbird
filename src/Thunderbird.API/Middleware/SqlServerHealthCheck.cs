using Microsoft.Extensions.Diagnostics.HealthChecks;
using Thunderbird.Domain.Interfaces;

namespace Thunderbird.API.Middleware {
    public class SqlServerHealthCheck : IHealthCheck {
        private readonly IBaseRepository _baseRepository;
        public SqlServerHealthCheck(IBaseRepository baseRepository) {
            _baseRepository = baseRepository;
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default) {
            try {
                using var connection = _baseRepository.GetConnection();
                return Task.FromResult(HealthCheckResult.Healthy());
            }
            catch (Exception ex) {
                return Task.FromResult(HealthCheckResult.Unhealthy("Unable to connect to SQL Server.", ex));
            }
        }
    }
}
