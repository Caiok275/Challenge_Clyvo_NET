using Challenge_Clyvo_NET.Data;
using Challenge_Clyvo_NET.HealthChecks;
using Challenge_Clyvo_NET.Middleware;
using Challenge_Clyvo_NET.Telemetry;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;

// Logger "bootstrap": captura logs (inclusive erros) durante a própria
// inicialização da aplicação, antes do host e do container de DI estarem
// prontos. É substituído pelo logger definitivo (configurado via
// appsettings.json) assim que builder.Host.UseSerilog(...) roda, abaixo.
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Iniciando a aplicação Challenge_Clyvo_NET");

    var builder = WebApplication.CreateBuilder(args);

    // Troca o provedor de logging padrão do ASP.NET Core pelo Serilog. A
    // configuração (sinks, níveis mínimos, etc.) vem da seção "Serilog" do
    // appsettings.json / appsettings.{Environment}.json.
    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext());

    var connectionString = builder.Configuration.GetConnectionString("OracleConnection");

    builder.Services.AddDbContext<AppDbContext>(options =>options.UseOracle(connectionString, b => b.UseOracleSQLCompatibility(OracleSQLCompatibility.DatabaseVersion19)));

    builder.Services.AddControllers();

    builder.Services.AddEndpointsApiExplorer();

    builder.Services.AddSwaggerGen();

    builder.Services.AddHealthChecks()
        .AddCheck<OracleDBHealth>("OracleDB");

    // ----- OpenTelemetry: distributed tracing + métricas de desempenho -----
    var otelServiceName = builder.Configuration["OpenTelemetry:ServiceName"] ?? builder.Environment.ApplicationName;
    var otlpEndpoint = builder.Configuration["OpenTelemetry:OtlpEndpoint"];

    // IMeterFactory usado pela classe AppMetrics para criar o Meter via DI.
    builder.Services.AddMetrics();
    builder.Services.AddSingleton<AppMetrics>();

    builder.Services.AddOpenTelemetry()
        .ConfigureResource(resource => resource
            .AddService(serviceName: otelServiceName, serviceVersion: "1.0.0")
            .AddAttributes(new Dictionary<string, object>
            {
                ["deployment.environment"] = builder.Environment.EnvironmentName
            }))
        .WithTracing(tracing =>
        {
            tracing
                // Spans manuais criados via AppActivitySource.Instance.StartActivity(...)
                .AddSource(AppActivitySource.Name)
                // Rastreia a requisição HTTP recebida (camada de entrada/API)
                .AddAspNetCoreInstrumentation(options =>
                {
                    options.RecordException = true;
                    // Não rastreia o próprio endpoint de métricas, para não poluir os traces
                    options.Filter = httpContext => !httpContext.Request.Path.StartsWithSegments("/metrics");
                })
                // Rastreia chamadas HTTP de saída (ex.: se a API vier a consumir outro serviço)
                .AddHttpClientInstrumentation()
                // Rastreia as consultas feitas via EF Core (camada de acesso a dados / Oracle).
                // EnrichWithIDbCommand adiciona o texto do SQL executado como tag do span.
                .AddEntityFrameworkCoreInstrumentation(efOptions =>
                {
                    efOptions.EnrichWithIDbCommand = (activity, command) =>
                    {
                        activity.SetTag("db.statement", command.CommandText);
                    };
                });

            if (!string.IsNullOrWhiteSpace(otlpEndpoint))
            {
                // Exporta para um coletor OTLP (Jaeger, Tempo, OpenTelemetry Collector, etc.)
                tracing.AddOtlpExporter(otlp => otlp.Endpoint = new Uri(otlpEndpoint));
            }
            else if (builder.Environment.IsDevelopment())
            {
                // Sem coletor configurado: imprime os traces no console, só em Development
                tracing.AddConsoleExporter();
            }
        })
        .WithMetrics(metrics =>
        {
            metrics
                // Métricas customizadas (tempo de resposta e taxa de erros) definidas em AppMetrics
                .AddMeter(AppMetrics.MeterName)
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddRuntimeInstrumentation()
                // Expõe as métricas em /metrics, no formato Prometheus
                .AddPrometheusExporter();

            if (!string.IsNullOrWhiteSpace(otlpEndpoint))
            {
                metrics.AddOtlpExporter(otlp => otlp.Endpoint = new Uri(otlpEndpoint));
            }
        });

    var app = builder.Build();

    // Deve vir antes de tudo para que o CorrelationId esteja disponível
    // em todo o restante do pipeline (Swagger, health checks, controllers etc.)
    app.UseCorrelationId();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.MapHealthChecks("/health", new HealthCheckOptions
    {
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });

    // Métricas de desempenho (tempo de resposta, taxa de erros, runtime, etc.)
    // expostas no formato Prometheus para scraping.
    app.MapPrometheusScrapingEndpoint();

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception ex) when (ex is not HostAbortedException)
{
    // HostAbortedException é lançada de propósito pelas ferramentas de
    // design-time do EF Core (ex.: "dotnet ef migrations add"), que
    // constroem o host só para inspecionar os serviços e depois abortam.
    // Não é um erro real da aplicação, então não deve ser logado como Fatal.
    Log.Fatal(ex, "A aplicação Challenge_Clyvo_NET terminou inesperadamente durante a inicialização");
}
finally
{
    Log.CloseAndFlush();
}

// Necessário para que o WebApplicationFactory<Program>, usado nos testes de
// integração, consiga enxergar e instanciar esta classe Program gerada pelos
// top-level statements (que por padrão é 'internal').
public partial class Program { }