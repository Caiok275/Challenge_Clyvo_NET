using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Challenge_Clyvo_NET.IntegrationTests.Fixtures;
using Xunit;

namespace Challenge_Clyvo_NET.IntegrationTests.Controllers
{
    /// Testes sobem a API real
    /// via WebApplicationFactory (com EF Core InMemory no lugar do Oracle) e
    /// validam requisições HTTP completas
    /// 
    [Collection("Integration Tests")]
    public class PessoaEndpointsTests
    {
        private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

        private readonly HttpClient _client;

        public PessoaEndpointsTests(ApiWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        private record PessoaResponse(int Id, string Nome, string Cpf, DateTime DataNascimento);

        [Fact]
        public async Task Post_NovaPessoaValida_RetornaCreatedComPessoaPersistida()
        {
            // Arrange
            var cpf = GerarCpfUnico();
            var novaPessoa = new
            {
                nome = "Maria da Silva",
                cpf,
                dataNascimento = "1990-05-20T00:00:00"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/pessoas", novaPessoa);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var pessoaCriada = await response.Content.ReadFromJsonAsync<PessoaResponse>(JsonOptions);
            Assert.NotNull(pessoaCriada);
            Assert.Equal("Maria da Silva", pessoaCriada!.Nome);
            Assert.Equal(cpf, pessoaCriada.Cpf);
            Assert.True(pessoaCriada.Id > 0);
        }

        [Fact]
        public async Task GetById_AposCriarPessoa_RetornaOkComOsMesmosDados()
        {
            // Arrange
            var cpf = GerarCpfUnico();
            var novaPessoa = new { nome = "João Pereira", cpf, dataNascimento = "1985-01-10T00:00:00" };
            var postResponse = await _client.PostAsJsonAsync("/api/pessoas", novaPessoa);
            var pessoaCriada = await postResponse.Content.ReadFromJsonAsync<PessoaResponse>(JsonOptions);

            // Act
            var response = await _client.GetAsync($"/api/pessoas/{pessoaCriada!.Id}");

            // Assert
            response.EnsureSuccessStatusCode();
            var pessoa = await response.Content.ReadFromJsonAsync<PessoaResponse>(JsonOptions);
            Assert.Equal("João Pereira", pessoa!.Nome);
            Assert.Equal(cpf, pessoa.Cpf);
        }

        [Fact]
        public async Task GetById_IdInexistente_RetornaNotFound()
        {
            // Act
            var response = await _client.GetAsync("/api/pessoas/999999");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Get_AposCriarPessoa_RetornaListaContendoAPessoaCriada()
        {
            // Arrange
            var cpf = GerarCpfUnico();
            var novaPessoa = new { nome = "Carla Menezes", cpf, dataNascimento = "1992-03-03T00:00:00" };
            await _client.PostAsJsonAsync("/api/pessoas", novaPessoa);

            // Act
            var response = await _client.GetAsync("/api/pessoas");

            // Assert
            response.EnsureSuccessStatusCode();
            var pessoas = await response.Content.ReadFromJsonAsync<List<PessoaResponse>>(JsonOptions);
            Assert.Contains(pessoas!, p => p.Cpf == cpf);
        }

        [Fact]
        public async Task Put_PessoaExistente_AtualizaERetornaNoContent()
        {
            // Arrange
            var cpfOriginal = GerarCpfUnico();
            var novaPessoa = new { nome = "Pedro Alves", cpf = cpfOriginal, dataNascimento = "1980-08-08T00:00:00" };
            var postResponse = await _client.PostAsJsonAsync("/api/pessoas", novaPessoa);
            var pessoaCriada = await postResponse.Content.ReadFromJsonAsync<PessoaResponse>(JsonOptions);

            var cpfAtualizado = GerarCpfUnico();
            var atualizacao = new { nome = "Pedro Alves Junior", cpf = cpfAtualizado, dataNascimento = "1981-09-09T00:00:00" };

            // Act
            var putResponse = await _client.PutAsJsonAsync($"/api/pessoas/{pessoaCriada!.Id}", atualizacao);

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, putResponse.StatusCode);

            var getResponse = await _client.GetAsync($"/api/pessoas/{pessoaCriada.Id}");
            var pessoaAtualizada = await getResponse.Content.ReadFromJsonAsync<PessoaResponse>(JsonOptions);
            Assert.Equal("Pedro Alves Junior", pessoaAtualizada!.Nome);
            Assert.Equal(cpfAtualizado, pessoaAtualizada.Cpf);
        }

        [Fact]
        public async Task Put_PessoaInexistente_RetornaNotFound()
        {
            // Arrange
            var atualizacao = new { nome = "Inexistente", cpf = GerarCpfUnico(), dataNascimento = "2000-01-01T00:00:00" };

            // Act
            var response = await _client.PutAsJsonAsync("/api/pessoas/999999", atualizacao);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Delete_PessoaExistente_RemoveERetornaNoContent()
        {
            // Arrange
            var cpf = GerarCpfUnico();
            var novaPessoa = new { nome = "Fernanda Costa", cpf, dataNascimento = "1993-04-04T00:00:00" };
            var postResponse = await _client.PostAsJsonAsync("/api/pessoas", novaPessoa);
            var pessoaCriada = await postResponse.Content.ReadFromJsonAsync<PessoaResponse>(JsonOptions);

            // Act
            var deleteResponse = await _client.DeleteAsync($"/api/pessoas/{pessoaCriada!.Id}");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

            var getResponse = await _client.GetAsync($"/api/pessoas/{pessoaCriada.Id}");
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }

        [Fact]
        public async Task Delete_PessoaInexistente_RetornaNotFound()
        {
            // Act
            var response = await _client.DeleteAsync("/api/pessoas/999999");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        /// Gera um CPF fictício e único por teste, para evitar conflito
        private static string GerarCpfUnico() => Guid.NewGuid().ToString("N")[..11];
    }
}
