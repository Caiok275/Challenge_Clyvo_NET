using Challenge_Clyvo_NET.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Challenge_Clyvo_NET.IntegrationTests.Fixtures
{
    /// Sobe a aplicação para os testes de
    /// integração, substituindo apenas o AppDbContext
    /// Nao utilizandoOracle DB ao ives disso ele usa o EF
    /// Core, isolado por execução (nome de banco único por instância da
    /// factory). Testando:
    /// model binding, middlewares (correlation id), controllers e EF Core —
    /// sem depender de infraestrutura externa.
    public class ApiWebApplicationFactory : WebApplicationFactory<Program>
    {
        private readonly string _databaseName = $"IntegrationTestsDb_{Guid.NewGuid()}";

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureServices(services =>
            {
                // Remove o registro original do AppDbContext
                var dbContextOptionsDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

                if (dbContextOptionsDescriptor != null)
                {
                    services.Remove(dbContextOptionsDescriptor);
                }

                // registra dados isolado, exclusivo desta instância da factory.
                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseInMemoryDatabase(_databaseName);
                });
            });
        }

        public AppDbContext CreateDbContext()
        {
            var scope = Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            context.Database.EnsureCreated();
            return context;
        }
    }
}
