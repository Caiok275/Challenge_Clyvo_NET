using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Challenge_Clyvo_NET.HealthChecks
{
    /// <summary>
    /// Publica o resultado de cada rodada de health checks no log estruturado,
    /// escolhendo o nível de log de acordo com o status agregado:
    /// Healthy -> Information, Degraded -> Warning, Unhealthy -> Error.
    /// Executa periodicamente (ver HealthCheckPublisherOptions.Period no Program.cs),
    /// independentemente de alguém estar chamando o endpoint /health.
    /// </summary>
    public class LoggingHealthCheckPublisher : IHealthCheckPublisher
    {
        private readonly ILogger<LoggingHealthCheckPublisher> _logger;

        public LoggingHealthCheckPublisher(ILogger<LoggingHealthCheckPublisher> logger)
        {
            _logger = logger;
        }

        public Task PublishAsync(HealthReport report, CancellationToken cancellationToken)
        {
            foreach (var entry in report.Entries)
            {
                var (name, result) = (entry.Key, entry.Value);

                switch (result.Status)
                {
                    case HealthStatus.Healthy:
                        _logger.LogInformation(
                            "Health check {CheckName} = Healthy ({ElapsedMs}ms) {Description}",
                            name, result.Duration.TotalMilliseconds, result.Description);
                        break;

                    case HealthStatus.Degraded:
                        _logger.LogWarning(
                            "Health check {CheckName} = Degraded ({ElapsedMs}ms) {Description}",
                            name, result.Duration.TotalMilliseconds, result.Description);
                        break;

                    case HealthStatus.Unhealthy:
                        _logger.LogError(
                            result.Exception,
                            "Health check {CheckName} = Unhealthy ({ElapsedMs}ms) {Description}",
                            name, result.Duration.TotalMilliseconds, result.Description);
                        break;
                }
            }

            _logger.LogInformation(
                "Resumo dos health checks: {Status} em {TotalElapsedMs}ms ({HealthyCount}/{TotalCount} saudáveis)",
                report.Status,
                report.TotalDuration.TotalMilliseconds,
                report.Entries.Count(e => e.Value.Status == HealthStatus.Healthy),
                report.Entries.Count);

            return Task.CompletedTask;
        }
    }
}
