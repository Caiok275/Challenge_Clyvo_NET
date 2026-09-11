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

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Iniciando a aplicação Challenge_Clyvo_NET");

    var builder = WebApplication.CreateBuilder(args);

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

    var otelServiceName = builder.Configuration["OpenTelemetry:ServiceName"] ?? builder.Environment.ApplicationName;
    var otlpEndpoint = builder.Configuration["OpenTelemetry:OtlpEndpoint"];

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
                .AddSource(AppActivitySource.Name)
                .AddAspNetCoreInstrumentation(options =>
                {
                    options.RecordException = true;
                    options.Filter = httpContext => !httpContext.Request.Path.StartsWithSegments("/metrics");
                })
                .AddHttpClientInstrumentation()
                .AddEntityFrameworkCoreInstrumentation(efOptions =>
                {
                    efOptions.EnrichWithIDbCommand = (activity, command) =>
                    {
                        activity.SetTag("db.statement", command.CommandText);
                    };
                });

            if (!string.IsNullOrWhiteSpace(otlpEndpoint))
            {
                tracing.AddOtlpExporter(otlp => otlp.Endpoint = new Uri(otlpEndpoint));
            }
            else if (builder.Environment.IsDevelopment())
            {
                tracing.AddConsoleExporter();
            }
        })
        .WithMetrics(metrics =>
        {
            metrics
                .AddMeter(AppMetrics.MeterName)
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddRuntimeInstrumentation()
                .AddPrometheusExporter();

            if (!string.IsNullOrWhiteSpace(otlpEndpoint))
            {
                metrics.AddOtlpExporter(otlp => otlp.Endpoint = new Uri(otlpEndpoint));
            }
        });

    var app = builder.Build();

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
.
    app.MapPrometheusScrapingEndpoint();

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception ex) when (ex is not HostAbortedException)
{
    Log.Fatal(ex, "A aplicação Challenge_Clyvo_NET fechou inesperadamente durante a inicialização");
}
finally
{
    Log.CloseAndFlush();
}

public partial class Program { }
