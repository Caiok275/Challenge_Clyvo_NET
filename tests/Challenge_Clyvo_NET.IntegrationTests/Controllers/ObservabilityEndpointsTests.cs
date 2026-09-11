using System.Net;
using Challenge_Clyvo_NET.IntegrationTests.Fixtures;
using Challenge_Clyvo_NET.Middleware;
using Xunit;

namespace Challenge_Clyvo_NET.IntegrationTests.Controllers
{
    /// <summary>
    /// Testes de integração para os recursos transversais da aplicação:
    /// health check (/health) e o middleware de Correlation ID.
    /// </summary>
    [Collection("Integration Tests")]
    public class ObservabilityEndpointsTests
    {
        private readonly HttpClient _client;

        public ObservabilityEndpointsTests(ApiWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Get_Health_RetornaOkQuandoOBancoInMemoryEstaAcessivel()
        {
            // Act
            var response = await _client.GetAsync("/health");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Get_QualquerEndpoint_DevolveHeaderDeCorrelationIdGeradoAutomaticamente()
        {
            // Act
            var response = await _client.GetAsync("/api/pessoas");

            // Assert
            Assert.True(response.Headers.Contains(CorrelationIdMiddleware.HeaderName));
            var correlationId = response.Headers.GetValues(CorrelationIdMiddleware.HeaderName).Single();
            Assert.False(string.IsNullOrWhiteSpace(correlationId));
        }

        [Fact]
        public async Task Get_ComCorrelationIdNoHeaderDeRequisicao_DevolveOMesmoValorNaResposta()
        {
            // Arrange
            var correlationIdEnviado = Guid.NewGuid().ToString();
            using var request = new HttpRequestMessage(HttpMethod.Get, "/api/pessoas");
            request.Headers.Add(CorrelationIdMiddleware.HeaderName, correlationIdEnviado);

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            var correlationIdRecebido = response.Headers.GetValues(CorrelationIdMiddleware.HeaderName).Single();
            Assert.Equal(correlationIdEnviado, correlationIdRecebido);
        }
    }
}
