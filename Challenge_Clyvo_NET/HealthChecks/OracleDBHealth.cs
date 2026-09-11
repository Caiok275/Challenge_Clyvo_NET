using Challenge_Clyvo_NET.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Challenge_Clyvo_NET.HealthChecks
{
    public class OracleDBHealth : IHealthCheck
    {
        private readonly AppDbContext _dbContext;

        public OracleDBHealth(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var canConnect = await _dbContext.Database.CanConnectAsync(cancellationToken);
                return canConnect
                    ? HealthCheckResult.Healthy("Database Oracle está saudável.")
                    : HealthCheckResult.Unhealthy("Não foi possível conectar ao Oracle DB.");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("Falha ao verificar o Oracle DB.", ex);
            }
        }
    }
}
