using Challenge_Clyvo_NET.Models;
using Xunit;

namespace Challenge_Clyvo_NET.UnitTests.Domain
{
    public class VeterinarioTests
    {
        [Fact]
        public void Constructor_DadosValidos_CriaVeterinarioComPropriedadesPreenchidas()
        {
            // Arrange
            var especialidade = "Cirurgia";
            var pessoaId = 7;

            // Act
            var veterinario = new Veterinario(especialidade, pessoaId);

            // Assert
            Assert.Equal(especialidade, veterinario.Especialidade);
            Assert.Equal(pessoaId, veterinario.PessoaId);
        }

        [Fact]
        public void Update_NovosDadosValidos_AtualizaPropriedadesDoVeterinario()
        {
            // Arrange
            var veterinario = new Veterinario("Cirurgia", 7);

            // Act
            veterinario.Update("Dermatologia", 9);

            // Assert
            Assert.Equal("Dermatologia", veterinario.Especialidade);
            Assert.Equal(9, veterinario.PessoaId);
        }
    }
}
