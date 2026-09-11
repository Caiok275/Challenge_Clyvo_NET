using Challenge_Clyvo_NET.Models;
using Xunit;

namespace Challenge_Clyvo_NET.UnitTests.Domain
{
    public class ResponsavelTests
    {
        [Fact]
        public void Constructor_DadosValidos_CriaResponsavelComPropriedadesPreenchidas()
        {
            // Arrange
            var endereco = "Rua das Flores, 123";
            var pessoaId = 5;

            // Act
            var responsavel = new Responsavel(endereco, pessoaId);

            // Assert
            Assert.Equal(endereco, responsavel.Endereco);
            Assert.Equal(pessoaId, responsavel.PessoaId);
        }

        [Fact]
        public void Update_NovosDadosValidos_AtualizaPropriedadesDoResponsavel()
        {
            // Arrange
            var responsavel = new Responsavel("Rua das Flores, 123", 5);

            // Act
            responsavel.Update("Avenida Central, 900", 8);

            // Assert
            Assert.Equal("Avenida Central, 900", responsavel.Endereco);
            Assert.Equal(8, responsavel.PessoaId);
        }
    }
}
