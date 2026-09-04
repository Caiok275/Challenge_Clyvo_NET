namespace Challenge_Clyvo_NET.Models
{
    public class Animal
    {
        public int Id { get; private set; }

        public string Nome { get; private set; }

        public int Idade { get; private set; }

        public string Especie { get; private set; }

        public string Raca { get; private set; }

        public string Sexo { get; private set; }

        public DateTime DataNascimento { get; private set; }

        public double Peso { get; private set; }

        public int ResponsavelId { get; private set; }

        public Responsavel? Responsavel { get; private set; }

        protected Animal() { }

        public Animal (string nome, int idade, string especie, string raca, string sexo, DateTime dataNascimento, double peso, int responsavelId)
        {
            Nome = nome;
            Idade = idade;
            Especie = especie;
            Raca = raca;
            Sexo = sexo;
            DataNascimento = dataNascimento;
            Peso = peso;
            ResponsavelId = responsavelId;
        }

        public void Update(string nome, int idade, string especie, string raca, string sexo, DateTime dataNascimento, double peso, int responsavelId)
        {
            Nome = nome;
            Idade = idade;
            Especie = especie;
            Raca = raca;
            Sexo = sexo;
            DataNascimento = dataNascimento;
            Peso = peso;
            ResponsavelId = responsavelId;
        }
    }
}
