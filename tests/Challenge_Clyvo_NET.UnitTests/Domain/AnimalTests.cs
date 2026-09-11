using Challenge_Clyvo_NET.Models;
using Xunit;

namespace Challenge_Clyvo_NET.UnitTests.Domain
{
    public class AnimalTests
    {
        [Fact]
        public void Constructor_DadosValidos_CriaAnimalComPropriedadesPreenchidas()
        {
            // Arrange
            var nome = "Rex";
            var idade = 3;
            var especie = "Canino";
            var raca = "Labrador";
            var sexo = "M";
            var dataNascimento = new DateTime(2022, 1, 5);
            var peso = 28.5;
            var responsavelId = 10;

            // Act
            var animal = new Animal(nome, idade, especie, raca, sexo, dataNascimento, peso, responsavelId);

            // Assert
            Assert.Equal(nome, animal.Nome);
            Assert.Equal(idade, animal.Idade);
            Assert.Equal(especie, animal.Especie);
            Assert.Equal(raca, animal.Raca);
            Assert.Equal(sexo, animal.Sexo);
            Assert.Equal(dataNascimento, animal.DataNascimento);
            Assert.Equal(peso, animal.Peso);
            Assert.Equal(responsavelId, animal.ResponsavelId);
        }

        [Fact]
        public void Update_NovosDadosValidos_AtualizaPropriedadesDoAnimal()
        {
            // Arrange
            var animal = new Animal("Rex", 3, "Canino", "Labrador", "M", new DateTime(2022, 1, 5), 28.5, 10);

            // Act
            animal.Update("Rex Junior", 4, "Canino", "Vira-lata", "M", new DateTime(2021, 12, 20), 30.2, 11);

            // Assert
            Assert.Equal("Rex Junior", animal.Nome);
            Assert.Equal(4, animal.Idade);
            Assert.Equal("Vira-lata", animal.Raca);
            Assert.Equal(30.2, animal.Peso);
            Assert.Equal(11, animal.ResponsavelId);
        }
    }
}
