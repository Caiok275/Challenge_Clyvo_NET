using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text.Json;

namespace Challenge_Clyvo_NET.HealthChecks
{
    /// <summary>
    /// Serializa o resultado do HealthReport em um JSON legível, contendo
    /// status geral, duração total e o detalhe de cada check individual.
    /// </summary>
    public static class HealthCheckJsonResponseWriter
    {
        private static readonly JsonSerializerOptions SerializerOptions = new()
        {
            WriteIndented = true
        };

        public static Task WriteResponse(HttpContext context, HealthReport report)
        {
            context.Response.ContentType = "application/json; charset=utf-8";

            var payload = new
            {
                status = report.Status.ToString(),
                totalDurationMs = report.TotalDuration.TotalMilliseconds,
                timestampUtc = DateTime.UtcNow,
                correlationId = context.Items["CorrelationId"]?.ToString(),
                checks = report.Entries.Select(entry => new
                {
                    name = entry.Key,
                    status = entry.Value.Status.ToString(),
                    description = entry.Value.Description,
                    durationMs = entry.Value.Duration.TotalMilliseconds,
                    tags = entry.Value.Tags,
                    data = entry.Value.Data,
                    exception = entry.Value.Exception?.Message
                })
            };

            return context.Response.WriteAsync(JsonSerializer.Serialize(payload, SerializerOptions));
        }
    }
}
