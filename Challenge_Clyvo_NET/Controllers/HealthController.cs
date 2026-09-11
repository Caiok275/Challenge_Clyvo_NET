using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Challenge_Clyvo_NET.Controllers
{
    /// <summary>
    /// Expõe o resultado dos health checks (mesmo resultado do endpoint
    /// /health, mapeado diretamente pelo middleware de Health Checks) como
    /// uma action de controller comum — e, por isso, visível no Swagger.
    ///
    /// O endpoint "cru" em /health continua existindo e é o recomendado
    /// para uso por orquestradores (Docker/Kubernetes, load balancers, etc.),
    /// já que é mais leve. Esta rota aqui é só para conveniência de quem
    /// está explorando a API pelo Swagger.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly HealthCheckService _healthCheckService;

        public HealthController(HealthCheckService healthCheckService)
        {
            _healthCheckService = healthCheckService;
        }

        /// <summary>
        /// Executa todos os health checks registrados (hoje, apenas o
        /// "OracleDB") e devolve o status agregado.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<IActionResult> Get()
        {
            var report = await _healthCheckService.CheckHealthAsync();

            var resposta = new
            {
                status = report.Status.ToString(),
                totalDuration = report.TotalDuration,
                entries = report.Entries.ToDictionary(
                    e => e.Key,
                    e => new
                    {
                        status = e.Value.Status.ToString(),
                        description = e.Value.Description,
                        duration = e.Value.Duration,
                        exception = e.Value.Exception?.Message
                    })
            };

            return report.Status == HealthStatus.Healthy
                ? Ok(resposta)
                : StatusCode(StatusCodes.Status503ServiceUnavailable, resposta);
        }
    }
}
