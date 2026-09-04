using Challenge_Clyvo_NET.Models;
using Microsoft.EntityFrameworkCore;

namespace Challenge_Clyvo_NET.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options) {}

        public DbSet<Pessoa> Pessoas { get; set; }

        public DbSet<Responsavel> Responsaveis { get; set; }

        public DbSet<Veterinario> Veterinarios { get; set; }

        public DbSet<Animal> Animais { get; set; }

        public DbSet<Contato> Contatos { get; set; }

        public DbSet<Consulta> Consultas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Pessoa>().ToTable("T_NET_PESSOA");
            modelBuilder.Entity<Pessoa>().Property(p => p.Id).HasColumnName("ID_PESSOA");
            modelBuilder.Entity<Pessoa>().Property(p => p.Nome).HasColumnName("NM_PESSOA");
            modelBuilder.Entity<Pessoa>().Property(p => p.Cpf).HasColumnName("CPF");
            modelBuilder.Entity<Pessoa>().Property(p => p.DataNascimento).HasColumnName("DT_NASCIMENTO");

            modelBuilder.Entity<Responsavel>().ToTable("T_NET_RESPONSAVEL");
            modelBuilder.Entity<Responsavel>().Property(r => r.Id).HasColumnName("ID_RESPONSAVEL");
            modelBuilder.Entity<Responsavel>().Property(r => r.Endereco).HasColumnName("ENDERECO");
            modelBuilder.Entity<Responsavel>().Property(r => r.PessoaId).HasColumnName("ID_PESSOA");

            modelBuilder.Entity<Veterinario>().ToTable("T_NET_VETERINARIO");
            modelBuilder.Entity<Veterinario>().Property(v => v.Id).HasColumnName("ID_VETERINARIO");
            modelBuilder.Entity<Veterinario>().Property(v => v.Especialidade).HasColumnName("ESPECIALIDADE");
            modelBuilder.Entity<Veterinario>().Property(v => v.PessoaId).HasColumnName("ID_PESSOA");

            modelBuilder.Entity<Animal>().ToTable("T_NET_ANIMAL");
            modelBuilder.Entity<Animal>().Property(a => a.Id).HasColumnName("ID_ANIMAL");
            modelBuilder.Entity<Animal>().Property(a => a.Nome).HasColumnName("NM_ANIMAL");
            modelBuilder.Entity<Animal>().Property(a => a.Idade).HasColumnName("IDADE");
            modelBuilder.Entity<Animal>().Property(a => a.Especie).HasColumnName("ESPECIE");
            modelBuilder.Entity<Animal>().Property(a => a.Raca).HasColumnName("RACA");
            modelBuilder.Entity<Animal>().Property(a => a.Sexo).HasColumnName("SEXO");
            modelBuilder.Entity<Animal>().Property(a => a.DataNascimento).HasColumnName("DT_NASCIMENTO_ANIMAL");
            modelBuilder.Entity<Animal>().Property(a => a.Peso).HasColumnName("PESO").HasPrecision(4, 2);
            modelBuilder.Entity<Animal>().Property(a => a.ResponsavelId).HasColumnName("ID_RESPONSAVEL");

            modelBuilder.Entity<Contato>().ToTable("T_NET_CONTATO");
            modelBuilder.Entity<Contato>().Property(c => c.Id).HasColumnName("ID_CONTATO");
            modelBuilder.Entity<Contato>().Property(c => c.Numero).HasColumnName("NUM_CONTATO");
            modelBuilder.Entity<Contato>().Property(c => c.Email).HasColumnName("EMAIL_CONTATO");
            modelBuilder.Entity<Contato>().Property(c => c.PessoaId).HasColumnName("ID_PESSOA");

            modelBuilder.Entity<Consulta>().ToTable("T_NET_CONSULTA");
            modelBuilder.Entity<Consulta>().Property(c => c.Id).HasColumnName("ID_CONSULTA");
            modelBuilder.Entity<Consulta>().Property(c => c.DataAgendamento).HasColumnName("DT_AGENDAMENTO");
            modelBuilder.Entity<Consulta>().Property(c => c.DataConsulta).HasColumnName("DT_CONSULTA");
            modelBuilder.Entity<Consulta>().Property(c => c.AnimalId).HasColumnName("ID_ANIMAL");
            modelBuilder.Entity<Consulta>().Property(c => c.VeterinarioId).HasColumnName("ID_VETERINARIO");

            base.OnModelCreating(modelBuilder);
        }
    }
}