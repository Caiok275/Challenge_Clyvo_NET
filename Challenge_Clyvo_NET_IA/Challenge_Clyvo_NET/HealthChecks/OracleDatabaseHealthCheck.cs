using Challenge_Clyvo_NET.Data;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Diagnostics;

namespace Challenge_Clyvo_NET.HealthChecks
{
    /// <summary>
    /// Verifica se é possível abrir uma conexão com o banco Oracle usado pela
    /// aplicação (AppDbContext). Marcada com as tags "ready" e "db" para poder
    /// ser filtrada separadamente do endpoint de liveness.
    /// </summary>
    public class OracleDatabaseHealthCheck : IHealthCheck
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<OracleDatabaseHealthCheck> _logger;

        public OracleDatabaseHealthCheck(AppDbContext dbContext, ILogger<OracleDatabaseHealthCheck> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                // CanConnectAsync abre e fecha uma conexão real com o Oracle,
                // sem depender de nenhuma tabela específica existir.
                var canConnect = await _dbContext.Database.CanConnectAsync(cancellationToken);
                stopwatch.Stop();

                var data = new Dictionary<string, object>
                {
                    ["responseTimeMs"] = stopwatch.ElapsedMilliseconds,
                    ["database"] = _dbContext.Database.GetDbConnection().Database ?? "unknown",
                };

                if (!canConnect)
                {
                    _logger.LogWarning(
                        "Health check do Oracle retornou indisponível após {ElapsedMs}ms",
                        stopwatch.ElapsedMilliseconds);

                    return HealthCheckResult.Unhealthy(
                        "Não foi possível conectar ao banco de dados Oracle.",
                        data: data);
                }

                if (stopwatch.ElapsedMilliseconds > 1000)
                {
                    _logger.LogWarning(
                        "Conexão com o Oracle bem-sucedida, porém lenta ({ElapsedMs}ms)",
                        stopwatch.ElapsedMilliseconds);

                    return HealthCheckResult.Degraded(
                        $"Conexão estabelecida, mas latência elevada ({stopwatch.ElapsedMilliseconds}ms).",
                        data: data);
                }

                _logger.LogInformation(
                    "Health check do Oracle OK em {ElapsedMs}ms",
                    stopwatch.ElapsedMilliseconds);

                return HealthCheckResult.Healthy("Conexão com o Oracle estabelecida com sucesso.", data);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();

                _logger.LogError(ex,
                    "Falha ao executar health check do banco Oracle após {ElapsedMs}ms",
                    stopwatch.ElapsedMilliseconds);

                return HealthCheckResult.Unhealthy(
                    "Exceção ao tentar conectar ao banco de dados Oracle.",
                    exception: ex,
                    data: new Dictionary<string, object>
                    {
                        ["responseTimeMs"] = stopwatch.ElapsedMilliseconds
                    });
            }
        }
    }
}
