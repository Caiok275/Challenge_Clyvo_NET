namespace Challenge_Clyvo_NET.Models
{
    public class Responsavel
    {
        public int Id { get; private set; }

        public string Endereco { get; private set; }

        public int PessoaId { get; private set; }

        public Pessoa? Pessoa { get; private set; }

        protected Responsavel() { }

        public Responsavel(string endereco, int pessoaId)
        {
            Endereco = endereco;
            PessoaId = pessoaId;
        }

        public void Update(string endereco, int pessoaId)
        {
            Endereco = endereco;
            PessoaId = pessoaId;
        }
    }
}
