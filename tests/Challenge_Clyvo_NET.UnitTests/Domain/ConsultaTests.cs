using Challenge_Clyvo_NET.Models;
using Xunit;

namespace Challenge_Clyvo_NET.UnitTests.Domain
{
    public class ConsultaTests
    {
        [Fact]
        public void Constructor_DadosValidos_CriaConsultaComPropriedadesPreenchidas()
        {
            // Arrange
            var dataAgendamento = new DateTime(2026, 9, 1, 10, 0, 0);
            var dataConsulta = new DateTime(2026, 9, 10, 14, 30, 0);
            var animalId = 1;
            var veterinarioId = 2;

            // Act
            var consulta = new Consulta(dataAgendamento, dataConsulta, animalId, veterinarioId);

            // Assert
            Assert.Equal(dataAgendamento, consulta.DataAgendamento);
            Assert.Equal(dataConsulta, consulta.DataConsulta);
            Assert.Equal(animalId, consulta.AnimalId);
            Assert.Equal(veterinarioId, consulta.VeterinarioId);
        }

        [Fact]
        public void Update_NovosDadosValidos_AtualizaPropriedadesDaConsulta()
        {
            // Arrange
            var consulta = new Consulta(
                new DateTime(2026, 9, 1, 10, 0, 0),
                new DateTime(2026, 9, 10, 14, 30, 0),
                animalId: 1,
                veterinarioId: 2);

            var novaDataAgendamento = new DateTime(2026, 10, 1, 9, 0, 0);
            var novaDataConsulta = new DateTime(2026, 10, 15, 16, 0, 0);

            // Act
            consulta.Update(novaDataAgendamento, novaDataConsulta, animalId: 4, veterinarioId: 5);

            // Assert
            Assert.Equal(novaDataAgendamento, consulta.DataAgendamento);
            Assert.Equal(novaDataConsulta, consulta.DataConsulta);
            Assert.Equal(4, consulta.AnimalId);
            Assert.Equal(5, consulta.VeterinarioId);
        }
    }
}
