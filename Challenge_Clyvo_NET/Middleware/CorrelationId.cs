using System.Diagnostics;
using Challenge_Clyvo_NET.Telemetry;

namespace Challenge_Clyvo_NET.Middleware
{
    /// <summary>
    /// Garante que toda requisição tenha um Correlation ID:
    /// reaproveita o valor recebido no header (se o cliente já enviou um),
    /// ou gera um novo Guid caso contrário. O valor é devolvido no header
    /// de resposta, injetado em um escopo de log (ILogger.BeginScope) e
    /// anexado ao span de trace atual (Activity), para que logs, traces e
    /// métricas de uma mesma requisição fiquem correlacionados pelo mesmo id.
    /// Também registra, para cada requisição, o tempo de resposta e se ela
    /// terminou em erro, via <see cref="AppMetrics"/>.
    /// </summary>
    public class CorrelationIdMiddleware
    {
        public const string HeaderName = "X-Correlation-Id";

        private readonly RequestDelegate _next;
        private readonly ILogger<CorrelationIdMiddleware> _logger;
        private readonly AppMetrics _metrics;

        public CorrelationIdMiddleware(RequestDelegate next, ILogger<CorrelationIdMiddleware> logger, AppMetrics metrics)
        {
            _next = next;
            _logger = logger;
            _metrics = metrics;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var correlationId = ResolveCorrelationId(context);

            // Disponibiliza o valor para o resto do pipeline (controllers, outros middlewares, etc.)
            context.Items[HeaderName] = correlationId;

            // O ASP.NET Core já cria o Activity (span) da requisição antes deste
            // middleware rodar (via OpenTelemetry.Instrumentation.AspNetCore).
            // Aqui só enriquecemos esse span com o correlation id: como tag,
            // para aparecer no trace, e como Baggage, para propagar automaticamente
            // via header (a especificação W3C) caso esta API chame outro serviço.
            var activity = Activity.Current;
            activity?.SetTag("app.correlation_id", correlationId);
            activity?.SetBaggage("correlation.id", correlationId);

            context.Response.OnStarting(() =>
            {
                if (!context.Response.Headers.ContainsKey(HeaderName))
                {
                    context.Response.Headers[HeaderName] = correlationId;
                }
                return Task.CompletedTask;
            });

            var stopwatch = Stopwatch.StartNew();
            var route = context.Request.Path.Value ?? "unknown";

            // Todo log emitido dentro deste using carrega o CorrelationId e o TraceId
            // automaticamente (funciona com provedores de log estruturado).
            using (_logger.BeginScope(new Dictionary<string, object>
            {
                ["CorrelationId"] = correlationId,
                ["TraceId"] = activity?.TraceId.ToString() ?? string.Empty
            }))
            {
                _logger.LogInformation("Requisição iniciada {Method} {Path}", context.Request.Method, context.Request.Path);

                try
                {
                    await _next(context);
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();
                    activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                    _logger.LogError(ex, "Requisição {Method} {Path} falhou com exceção não tratada", context.Request.Method, context.Request.Path);
                    _metrics.RecordRequest(context.Request.Method, route, StatusCodes.Status500InternalServerError, stopwatch.Elapsed.TotalMilliseconds);
                    throw;
                }

                stopwatch.Stop();
                _metrics.RecordRequest(context.Request.Method, route, context.Response.StatusCode, stopwatch.Elapsed.TotalMilliseconds);

                _logger.LogInformation(
                    "Requisição finalizada {Method} {Path} -> {StatusCode} em {ElapsedMs}ms",
                    context.Request.Method, context.Request.Path, context.Response.StatusCode, stopwatch.Elapsed.TotalMilliseconds);
            }
        }

        private static string ResolveCorrelationId(HttpContext context)
        {
            if (context.Request.Headers.TryGetValue(HeaderName, out var existing) &&
                !string.IsNullOrWhiteSpace(existing))
            {
                return existing.ToString();
            }

            return Guid.NewGuid().ToString();
        }
    }

    /// <summary>
    /// Extensão para registrar o middleware de forma legível no Program.cs.
    /// </summary>
    public static class CorrelationIdMiddlewareExtensions
    {
        public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder app)
        {
            return app.UseMiddleware<CorrelationIdMiddleware>();
        }
    }
}
