using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace Challenge_Clyvo_NET.Telemetry
{
    /// <summary>
    /// Métricas de desempenho da aplicação: tempo de resposta e taxa de erros
    /// das requisições HTTP, expostas via OpenTelemetry (endpoint /metrics no
    /// formato Prometheus e, se configurado, também via OTLP).
    ///
    /// Complementa (não substitui) as métricas nativas do ASP.NET Core
    /// (http.server.request.duration, etc.), fornecendo nomes e contadores
    /// explícitos, fáceis de montar em um dashboard.
    /// </summary>
    public class AppMetrics
    {
        public const string MeterName = "Challenge_Clyvo_NET";

        private readonly Histogram<double> _requestDuration;
        private readonly Counter<long> _requestsTotal;
        private readonly Counter<long> _requestsErrors;

        public AppMetrics(IMeterFactory meterFactory)
        {
            var meter = meterFactory.Create(MeterName);

            _requestDuration = meter.CreateHistogram<double>(
                name: "app.http.server.request.duration",
                unit: "ms",
                description: "Tempo de resposta das requisições HTTP, em milissegundos.");

            _requestsTotal = meter.CreateCounter<long>(
                name: "app.http.server.requests.total",
                description: "Total de requisições HTTP processadas.");

            _requestsErrors = meter.CreateCounter<long>(
                name: "app.http.server.requests.errors",
                description: "Total de requisições HTTP finalizadas com erro (status >= 400).");
        }

        /// <summary>
        /// Registra o resultado de uma requisição: incrementa o total, o tempo
        /// de resposta e, se o status indicar erro, o contador de erros —
        /// todos com as mesmas tags, para permitir filtrar/agrupar por rota,
        /// método e status no backend de métricas.
        /// </summary>
        public void RecordRequest(string method, string route, int statusCode, double elapsedMilliseconds)
        {
            var tags = new TagList
            {
                { "http.request.method", method },
                { "http.route", route },
                { "http.response.status_code", statusCode }
            };

            _requestDuration.Record(elapsedMilliseconds, tags);
            _requestsTotal.Add(1, tags);

            if (statusCode >= 400)
            {
                _requestsErrors.Add(1, tags);
            }
        }
    }
}
