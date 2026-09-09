using Microsoft.Extensions.Diagnostics.HealthChecks;
using Oracle.ManagedDataAccess.Client;

namespace Challenge_Clyvo_NET.HealthChecks


{
    public class OracleDBHealth : IHealthCheck
    {
        private readonly string _connectionString;
        public OracleDBHealth(string connectionString)
        {
            _connectionString = connectionString;
        }
        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context, 
            CancellationToken cancellationToken = default)
        {
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    await connection.OpenAsync(cancellationToken);
                    return HealthCheckResult.Healthy("DataBase Oracle está saldavel.");
                }
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("DataBase Oracle não está saldavel.", ex);
            }
        }


    }
}
