using Xunit;

namespace Challenge_Clyvo_NET.IntegrationTests.Fixtures
{
    /// <summary>
    /// Define a coleção "Integration Tests": todas as classes marcadas com
    /// [Collection("Integration Tests")] compartilham a MESMA instância de
    /// <see cref="ApiWebApplicationFactory"/> (e, portanto, o mesmo servidor
    /// de teste em memória e o mesmo banco InMemory), em vez de subir uma
    /// aplicação nova para cada classe de teste. Isso reduz drasticamente o
    /// tempo total da suíte de integração.
    ///
    /// Esta classe não precisa de corpo: sua única função é carregar o
    /// atributo [CollectionDefinition] e implementar ICollectionFixture&lt;T&gt;.
    /// </summary>
    [CollectionDefinition("Integration Tests")]
    public class IntegrationTestCollection : ICollectionFixture<ApiWebApplicationFactory>
    {
    }
}
