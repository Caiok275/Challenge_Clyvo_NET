namespace Challenge_Clyvo_NET.Models
{
    public class Veterinario
    {
        public int Id { get; private set; }

        public string Especialidade { get; private set; }

        public int PessoaId { get; private set; }

        public Pessoa? Pessoa { get; private set; }

        protected Veterinario() { }

        public Veterinario(string especialidade, int pessoaId)
        {
            Especialidade = especialidade;
            PessoaId = pessoaId;
        }

        public void Update(string especialidade, int pessoaId)
        {
            Especialidade = especialidade;
            PessoaId = pessoaId;
        }
    }
}
