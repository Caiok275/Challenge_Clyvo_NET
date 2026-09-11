using Challenge_Clyvo_NET.Models;
using Xunit;

namespace Challenge_Clyvo_NET.UnitTests.Domain
{
    /// <summary>
    /// Testes unitários da entidade de domínio Pessoa.
    /// Não dependem de banco de dados, DI ou mocks: exercitam apenas as
    /// regras da própria classe (construtor e Update).
    /// </summary>
    public class PessoaTests
    {
        [Fact]
        public void Constructor_DadosValidos_CriaPessoaComPropriedadesPreenchidas()
        {
            // Arrange
            var nome = "Ana Souza";
            var cpf = "12345678900";
            var dataNascimento = new DateTime(1995, 3, 10);

            // Act
            var pessoa = new Pessoa(nome, cpf, dataNascimento);

            // Assert
            Assert.Equal(nome, pessoa.Nome);
            Assert.Equal(cpf, pessoa.Cpf);
            Assert.Equal(dataNascimento, pessoa.DataNascimento);
        }

        [Fact]
        public void Update_NovosDadosValidos_AtualizaPropriedadesDaPessoa()
        {
            // Arrange
            var pessoa = new Pessoa("Ana Souza", "12345678900", new DateTime(1995, 3, 10));
            var novoNome = "Ana Souza Lima";
            var novoCpf = "98765432100";
            var novaDataNascimento = new DateTime(1996, 6, 15);

            // Act
            pessoa.Update(novoNome, novoCpf, novaDataNascimento);

            // Assert
            Assert.Equal(novoNome, pessoa.Nome);
            Assert.Equal(novoCpf, pessoa.Cpf);
            Assert.Equal(novaDataNascimento, pessoa.DataNascimento);
        }

        [Fact]
        public void Constructor_NovaPessoa_IdPermaneceComValorPadraoAteSerPersistida()
        {
            // Arrange & Act
            // (o Id é atribuído pelo banco de dados; antes de persistir, deve
            // permanecer no valor padrão de int, já que o setter é privado)
            var pessoa = new Pessoa("Ana Souza", "12345678900", new DateTime(1995, 3, 10));

            // Assert
            Assert.Equal(0, pessoa.Id);
        }
    }
}
