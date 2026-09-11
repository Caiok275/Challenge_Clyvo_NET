using Challenge_Clyvo_NET.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Challenge_Clyvo_NET.IntegrationTests.Fixtures
{
    /// <summary>
    /// Sobe a aplicação real (Program.cs) em memória para os testes de
    /// integração, substituindo apenas o AppDbContext: em vez do provedor
    /// Oracle (que exigiria um banco real), usa o provedor InMemory do EF
    /// Core, isolado por execução (nome de banco único por instância da
    /// factory). Assim os testes validam o fluxo HTTP completo — roteamento,
    /// model binding, middlewares (correlation id), controllers e EF Core —
    /// sem depender de infraestrutura externa.
    /// </summary>
    public class ApiWebApplicationFactory : WebApplicationFactory<Program>
    {
        private readonly string _databaseName = $"IntegrationTestsDb_{Guid.NewGuid()}";

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureServices(services =>
            {
                // Remove o registro original do AppDbContext (configurado no
                // Program.cs para usar Oracle) ...
                var dbContextOptionsDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

                if (dbContextOptionsDescriptor != null)
                {
                    services.Remove(dbContextOptionsDescriptor);
                }

                // ... e registra novamente apontando para um banco InMemory
                // isolado, exclusivo desta instância da factory.
                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseInMemoryDatabase(_databaseName);
                });
            });
        }

        /// <summary>
        /// Cria um novo escopo de DI e devolve um AppDbContext apontando para
        /// o mesmo banco InMemory usado pela aplicação nos testes — útil para
        /// popular dados antes de uma requisição (Arrange) ou inspecionar o
        /// estado do banco depois dela (Assert).
        /// </summary>
        public AppDbContext CreateDbContext()
        {
            var scope = Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            context.Database.EnsureCreated();
            return context;
        }
    }
}
