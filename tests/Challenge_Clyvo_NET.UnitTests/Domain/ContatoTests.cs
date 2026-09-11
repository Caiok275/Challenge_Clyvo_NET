using Challenge_Clyvo_NET.Models;
using Xunit;

namespace Challenge_Clyvo_NET.UnitTests.Domain
{
    public class ContatoTests
    {
        [Fact]
        public void Constructor_DadosValidos_CriaContatoComPropriedadesPreenchidas()
        {
            // Arrange
            var numero = "11999998888";
            var email = "contato@exemplo.com";
            var pessoaId = 3;

            // Act
            var contato = new Contato(numero, email, pessoaId);

            // Assert
            Assert.Equal(numero, contato.Numero);
            Assert.Equal(email, contato.Email);
            Assert.Equal(pessoaId, contato.PessoaId);
        }

        [Fact]
        public void Update_NovosDadosValidos_AtualizaNumeroEEmailSemAlterarPessoaId()
        {
            // Arrange
            var contato = new Contato("11999998888", "contato@exemplo.com", 3);

            // Act
            contato.Update("11988887777", "novo@exemplo.com");

            // Assert
            Assert.Equal("11988887777", contato.Numero);
            Assert.Equal("novo@exemplo.com", contato.Email);
            Assert.Equal(3, contato.PessoaId); // Update não recebe pessoaId, então ele deve permanecer o mesmo
        }
    }
}
