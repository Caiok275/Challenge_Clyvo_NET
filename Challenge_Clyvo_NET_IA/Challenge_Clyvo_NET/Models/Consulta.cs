namespace Challenge_Clyvo_NET.Models
{
    public class Consulta
    {
        public int Id { get; private set; }
        public DateTime DataAgendamento { get; private set; }
        public DateTime DataConsulta { get; private set; }

        public int AnimalId { get; private set; }
        public Animal? Animal { get; private set; }

        public int VeterinarioId { get; private set; }
        public Veterinario? Veterinario { get; private set; }
        protected Consulta() { }

        public Consulta(DateTime dataAgendamento, DateTime dataConsulta, int animalId, int veterinarioId)
        {
            DataAgendamento = dataAgendamento;
            DataConsulta = dataConsulta;
            AnimalId = animalId;
            VeterinarioId = veterinarioId;
        }

        public void Update(DateTime dataAgendamento, DateTime dataConsulta, int animalId, int veterinarioId)
        {
            DataAgendamento = dataAgendamento;
            DataConsulta = dataConsulta;
            AnimalId = animalId;
            VeterinarioId = veterinarioId;
        }
    }
}