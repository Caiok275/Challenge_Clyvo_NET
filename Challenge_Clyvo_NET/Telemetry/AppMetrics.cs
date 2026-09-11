using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace Challenge_Clyvo_NET.Telemetry
{
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
