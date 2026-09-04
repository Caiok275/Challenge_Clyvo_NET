using Challenge_Clyvo_NET.Data;
using Challenge_Clyvo_NET.HealthChecks;
using Challenge_Clyvo_NET.Middleware;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;
using Serilog.Events;

// ------------------------------------------------------------------
// Bootstrap logger: captura qualquer erro que ocorra ANTES do host
// terminar de ser configurado (ex.: falha ao ler appsettings.json).
// ------------------------------------------------------------------
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Iniciando a aplicação Challenge_Clyvo_NET");

    var builder = WebApplication.CreateBuilder(args);

    // ------------------------------------------------------------------
    // Serilog: substitui o provider de logging padrão. Lê overrides de
    // appsettings.json (seção "Serilog") e enriquece cada evento com
    // contexto (CorrelationId via LogContext, nome da máquina, ambiente).
    // ------------------------------------------------------------------
    builder.Host.UseSerilog((context, services, loggerConfiguration) => loggerConfiguration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .Enrich.WithMachineName()
        .Enrich.WithEnvironmentName()
        .Enrich.WithProperty("Application", "Challenge_Clyvo_NET")
        .MinimumLevel.Information()
        .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
        .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
        .WriteTo.Console(
            outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] ({CorrelationId}) {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}")
        .WriteTo.File(
            path: "logs/log-.txt",
            rollingInterval: RollingInterval.Day,
            retainedFileCountLimit: 14,
            fileSizeLimitBytes: 50 * 1024 * 1024,
            rollOnFileSizeLimit: true,
            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] ({CorrelationId}) {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}"));

    var connectionString = builder.Configuration.GetConnectionString("OracleConnection");

    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseOracle(connectionString, b => b.UseOracleSQLCompatibility(OracleSQLCompatibility.DatabaseVersion19)));

    builder.Services.AddControllers();

    builder.Services.AddEndpointsApiExplorer();

    builder.Services.AddSwaggerGen();

    // ------------------------------------------------------------------
    // Health Checks (Microsoft.Extensions.Diagnostics.HealthChecks)
    //  - "self"            -> tag "live": não depende de nada externo,
    //                          usado para o probe de liveness.
    //  - "oracle_database" -> tags "ready" e "db": testa a conexão real
    //                          com o Oracle, usado para o probe de readiness.
    // ------------------------------------------------------------------
    builder.Services
        .AddHealthChecks()
        .AddCheck("self", () => HealthCheckResult.Healthy("A API está no ar."), tags: new[] { "live" })
        .AddCheck<OracleDatabaseHealthCheck>("oracle_database", tags: new[] { "ready", "db" });

    // Publica o resultado dos checks no log a cada 30s, mesmo sem ninguém
    // chamar o endpoint HTTP (útil para detectar degradação proativamente).
    builder.Services.Configure<HealthCheckPublisherOptions>(options =>
    {
        options.Delay = TimeSpan.FromSeconds(5);
        options.Period = TimeSpan.FromSeconds(30);
    });
    builder.Services.AddSingleton<IHealthCheckPublisher, LoggingHealthCheckPublisher>();

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    // Correlation ID precisa vir antes do request logging para que o
    // CorrelationId já esteja no LogContext quando a linha de resumo
    // da requisição for emitida.
    app.UseCorrelationId();

    // Log estruturado de cada requisição HTTP (método, path, status,
    // tempo de resposta), no nível Information (ou Warning/Error
    // automaticamente em caso de status 4xx/5xx ou exceção).
    app.UseSerilogRequestLogging(options =>
    {
        options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
            diagnosticContext.Set("CorrelationId", httpContext.Items["CorrelationId"]);
            diagnosticContext.Set("RemoteIp", httpContext.Connection.RemoteIpAddress?.ToString());
        };
    });

    app.UseAuthorization();

    app.MapControllers();

    // /health         -> visão completa (todos os checks), útil para dashboards.
    // /health/live    -> liveness probe: só confirma que o processo está de pé.
    // /health/ready   -> readiness probe: confirma dependências externas (Oracle).
    app.MapHealthChecks("/health", new HealthCheckOptions
    {
        ResponseWriter = HealthCheckJsonResponseWriter.WriteResponse
    });

    app.MapHealthChecks("/health/live", new HealthCheckOptions
    {
        Predicate = check => check.Tags.Contains("live"),
        ResponseWriter = HealthCheckJsonResponseWriter.WriteResponse
    });

    app.MapHealthChecks("/health/ready", new HealthCheckOptions
    {
        Predicate = check => check.Tags.Contains("ready"),
        ResponseWriter = HealthCheckJsonResponseWriter.WriteResponse
    });

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "A aplicação encerrou inesperadamente durante a inicialização");
}
finally
{
    Log.CloseAndFlush();
}
