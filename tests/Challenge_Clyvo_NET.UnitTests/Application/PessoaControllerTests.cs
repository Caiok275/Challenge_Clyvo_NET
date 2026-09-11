using Challenge_Clyvo_NET.Controllers;
using Challenge_Clyvo_NET.Data;
using Challenge_Clyvo_NET.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using MockQueryable.Moq;
using Xunit;

namespace Challenge_Clyvo_NET.UnitTests.Application
{
    /// Esse teste utiliza (PessoaController), com o
    /// AppDbContext.Pessoas mockado usando Moq/MockQueryable sem usar
    /// banco de dados real
    public class PessoaControllerTests
    {
        private static (AppDbContext Context, Mock<DbSet<Pessoa>> MockSet, List<Pessoa> Pessoas) CreateSut(
            IEnumerable<Pessoa>? seed = null)
        {
            var pessoas = seed?.ToList() ?? new List<Pessoa>();
            var mockSet = pessoas.BuildMockDbSet();

            mockSet.Setup(m => m.Add(It.IsAny<Pessoa>()))
                .Callback<Pessoa>(p => pessoas.Add(p));

            mockSet.Setup(m => m.Remove(It.IsAny<Pessoa>()))
                .Callback<Pessoa>(p => pessoas.Remove(p));

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new AppDbContext(options)
            {
                Pessoas = mockSet.Object
            };

            return (context, mockSet, pessoas);
        }

        [Fact]
        public async Task Get_QuandoExistemPessoasCadastradas_RetornaOkComTodasAsPessoas()
        {
            // Arrange
            var pessoasExistentes = new List<Pessoa>
            {
                new Pessoa("Ana Souza", "11111111111", new DateTime(1990, 1, 1)),
                new Pessoa("Bruno Lima", "22222222222", new DateTime(1985, 5, 20))
            };
            var (context, _, _) = CreateSut(pessoasExistentes);
            var controller = new PessoaController(context);

            // Act
            var resultado = await controller.Get();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado);
            var pessoas = Assert.IsAssignableFrom<IEnumerable<Pessoa>>(okResult.Value);
            Assert.Equal(2, pessoas.Count());
        }

        [Fact]
        public async Task Get_QuandoNaoHaPessoasCadastradas_RetornaOkComListaVazia()
        {
            // Arrange
            var (context, _, _) = CreateSut();
            var controller = new PessoaController(context);

            // Act
            var resultado = await controller.Get();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado);
            var pessoas = Assert.IsAssignableFrom<IEnumerable<Pessoa>>(okResult.Value);
            Assert.Empty(pessoas);
        }

        [Fact]
        public async Task GetById_IdExistente_RetornaOkComAPessoaCorrespondente()
        {
            // Arrange
            var pessoaEsperada = new Pessoa("Ana Souza", "11111111111", new DateTime(1990, 1, 1));
            var (context, mockSet, _) = CreateSut(new[] { pessoaEsperada });
            mockSet.Setup(m => m.FindAsync(It.IsAny<object[]>()))
                .ReturnsAsync(pessoaEsperada);
            var controller = new PessoaController(context);

            // Act
            var resultado = await controller.GetById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado);
            var pessoa = Assert.IsType<Pessoa>(okResult.Value);
            Assert.Equal(pessoaEsperada.Cpf, pessoa.Cpf);
        }

        [Fact]
        public async Task GetById_IdInexistente_RetornaNotFound()
        {
            // Arrange
            var (context, mockSet, _) = CreateSut();
            mockSet.Setup(m => m.FindAsync(It.IsAny<object[]>()))
                .ReturnsAsync((Pessoa?)null);
            var controller = new PessoaController(context);

            // Act
            var resultado = await controller.GetById(999);

            // Assert
            Assert.IsType<NotFoundResult>(resultado);
        }

        [Fact]
        public async Task GetByCpf_CpfExistente_RetornaOkComAPessoaCorrespondente()
        {
            // Arrange
            var pessoaEsperada = new Pessoa("Ana Souza", "11111111111", new DateTime(1990, 1, 1));
            var (context, _, _) = CreateSut(new[] { pessoaEsperada });
            var controller = new PessoaController(context);

            // Act
            var resultado = await controller.GetByCpf("11111111111");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado);
            var pessoa = Assert.IsType<Pessoa>(okResult.Value);
            Assert.Equal("Ana Souza", pessoa.Nome);
        }

        [Fact]
        public async Task GetByCpf_CpfInexistente_RetornaNotFound()
        {
            // Arrange
            var (context, _, _) = CreateSut();
            var controller = new PessoaController(context);

            // Act
            var resultado = await controller.GetByCpf("00000000000");

            // Assert
            Assert.IsType<NotFoundResult>(resultado);
        }

        [Fact]
        public async Task Post_PessoaValida_AdicionaEDevolveCreatedAtActionComAPessoa()
        {
            // Arrange
            var (context, mockSet, pessoas) = CreateSut();
            var novaPessoa = new Pessoa("Carlos Dias", "33333333333", new DateTime(1988, 7, 12));
            var controller = new PessoaController(context);

            // Act
            var resultado = await controller.Post(novaPessoa);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(resultado);
            Assert.Equal(nameof(PessoaController.GetById), createdResult.ActionName);
            Assert.Same(novaPessoa, createdResult.Value);
            mockSet.Verify(m => m.Add(It.Is<Pessoa>(p => p.Cpf == "33333333333")), Times.Once);
            Assert.Single(pessoas);
        }

        [Fact]
        public async Task Put_IdExistente_AtualizaDadosERetornaNoContent()
        {
            // Arrange
            var pessoaExistente = new Pessoa("Ana Souza", "11111111111", new DateTime(1990, 1, 1));
            var (context, mockSet, _) = CreateSut(new[] { pessoaExistente });
            mockSet.Setup(m => m.FindAsync(It.IsAny<object[]>()))
                .ReturnsAsync(pessoaExistente);
            var dadosAtualizados = new Pessoa("Ana Souza Lima", "44444444444", new DateTime(1991, 2, 2));
            var controller = new PessoaController(context);

            // Act
            var resultado = await controller.Put(1, dadosAtualizados);

            // Assert
            Assert.IsType<NoContentResult>(resultado);
            Assert.Equal("Ana Souza Lima", pessoaExistente.Nome);
            Assert.Equal("44444444444", pessoaExistente.Cpf);
        }

        [Fact]
        public async Task Put_IdInexistente_RetornaNotFound()
        {
            // Arrange
            var (context, mockSet, _) = CreateSut();
            mockSet.Setup(m => m.FindAsync(It.IsAny<object[]>()))
                .ReturnsAsync((Pessoa?)null);
            var dadosAtualizados = new Pessoa("Ana Souza Lima", "44444444444", new DateTime(1991, 2, 2));
            var controller = new PessoaController(context);

            // Act
            var resultado = await controller.Put(999, dadosAtualizados);

            // Assert
            Assert.IsType<NotFoundResult>(resultado);
        }

        [Fact]
        public async Task Delete_IdExistente_RemovePessoaERetornaNoContent()
        {
            // Arrange
            var pessoaExistente = new Pessoa("Ana Souza", "11111111111", new DateTime(1990, 1, 1));
            var (context, mockSet, pessoas) = CreateSut(new[] { pessoaExistente });
            mockSet.Setup(m => m.FindAsync(It.IsAny<object[]>()))
                .ReturnsAsync(pessoaExistente);
            var controller = new PessoaController(context);

            // Act
            var resultado = await controller.Delete(1);

            // Assert
            Assert.IsType<NoContentResult>(resultado);
            mockSet.Verify(m => m.Remove(pessoaExistente), Times.Once);
            Assert.Empty(pessoas);
        }

        [Fact]
        public async Task Delete_IdInexistente_RetornaNotFound()
        {
            // Arrange
            var (context, mockSet, _) = CreateSut();
            mockSet.Setup(m => m.FindAsync(It.IsAny<object[]>()))
                .ReturnsAsync((Pessoa?)null);
            var controller = new PessoaController(context);

            // Act
            var resultado = await controller.Delete(999);

            // Assert
            Assert.IsType<NotFoundResult>(resultado);
        }
    }
}
