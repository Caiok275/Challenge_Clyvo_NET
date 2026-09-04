using System.ComponentModel.DataAnnotations;

namespace Challenge_Clyvo_NET.Models
{
    public class Pessoa
    {
        public int Id { get; private set; }

        [Required]
        public string Nome { get; private set; }

        [Required]
        public string Cpf { get; private set; }

        public DateTime DataNascimento { get; private set; }

        protected Pessoa() { }

        public Pessoa(string nome, string cpf, DateTime dataNascimento)
        {
            Nome = nome;
            Cpf = cpf;
            DataNascimento = dataNascimento;
        }

        public void Update(string nome, string cpf, DateTime dataNascimento)
        {
            Nome = nome;
            Cpf = cpf;
            DataNascimento = dataNascimento;
        }
    }
}