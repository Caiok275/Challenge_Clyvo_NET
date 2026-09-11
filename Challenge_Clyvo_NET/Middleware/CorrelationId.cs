using System.Diagnostics;
using Challenge_Clyvo_NET.Telemetry;

namespace Challenge_Clyvo_NET.Middleware
{
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
            context.Items[HeaderName] = correlationId;

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
    public static class CorrelationIdMiddlewareExtensions
    {
        public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder app)
        {
            return app.UseMiddleware<CorrelationIdMiddleware>();
        }
    }
}
