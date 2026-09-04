namespace Challenge_Clyvo_NET.Models
{
    public class Contato
    {
        public int Id { get; private set; }
        public string Numero { get; private set; }
        public string Email { get; private set; }
        public int PessoaId { get; private set; }
        public Pessoa? Pessoa { get; private set; }

        protected Contato() { }

        public Contato(string numero, string email, int pessoaId)
        {
            Numero = numero;
            Email = email;
            PessoaId = pessoaId;
        }

        public void Update(string numero, string email)
        {
            Numero = numero;
            Email = email;
        }
    }
}
