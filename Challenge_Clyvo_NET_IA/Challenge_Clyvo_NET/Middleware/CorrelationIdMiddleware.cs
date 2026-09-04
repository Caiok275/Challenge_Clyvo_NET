using Serilog.Context;

namespace Challenge_Clyvo_NET.Middleware
{
    /// <summary>
    /// Garante que toda requisição tenha um Correlation ID (aceita o valor recebido
    /// no header, ou gera um novo), devolve o valor no header de resposta e o
    /// injeta no LogContext do Serilog para que TODOS os logs emitidos durante
    /// o processamento da requisição (incluindo o log de finalização gerado por
    /// UseSerilogRequestLogging) carreguem essa propriedade.
    /// </summary>
    public class CorrelationIdMiddleware
    {
        public const string HeaderName = "X-Correlation-ID";
        private const string HttpContextItemKey = "CorrelationId";

        private readonly RequestDelegate _next;
        private readonly ILogger<CorrelationIdMiddleware> _logger;

        public CorrelationIdMiddleware(RequestDelegate next, ILogger<CorrelationIdMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var correlationId = ResolveCorrelationId(context);

            context.Items[HttpContextItemKey] = correlationId;
            context.Response.OnStarting(() =>
            {
                context.Response.Headers[HeaderName] = correlationId;
                return Task.CompletedTask;
            });

            // PushProperty permanece ativo (via AsyncLocal) durante toda a
            // execução downstream, então qualquer log emitido por controllers,
            // health checks, EF Core, etc. dentro desta requisição herda o CorrelationId.
            using (LogContext.PushProperty("CorrelationId", correlationId))
            {
                _logger.LogInformation(
                    "Requisição recebida: {Method} {Path}",
                    context.Request.Method,
                    context.Request.Path);

                try
                {
                    await _next(context);

                    if (context.Response.StatusCode >= 500)
                    {
                        _logger.LogError(
                            "Requisição finalizada com erro de servidor: {Method} {Path} -> {StatusCode}",
                            context.Request.Method, context.Request.Path, context.Response.StatusCode);
                    }
                    else if (context.Response.StatusCode >= 400)
                    {
                        _logger.LogWarning(
                            "Requisição finalizada com erro de cliente: {Method} {Path} -> {StatusCode}",
                            context.Request.Method, context.Request.Path, context.Response.StatusCode);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "Exceção não tratada ao processar {Method} {Path}",
                        context.Request.Method, context.Request.Path);
                    throw;
                }
            }
        }

        private static string ResolveCorrelationId(HttpContext context)
        {
            if (context.Request.Headers.TryGetValue(HeaderName, out var incoming) &&
                !string.IsNullOrWhiteSpace(incoming))
            {
                return incoming.ToString();
            }

            return Guid.NewGuid().ToString("N");
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
