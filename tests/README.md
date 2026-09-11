# Testes Automatizados — Challenge_Clyvo_NET

## Estrutura

```
tests/
  Challenge_Clyvo_NET.UnitTests/         Testes unitários (xUnit + Moq)
    Domain/                              Entidades de domínio (Models)
    Application/                         Controllers (camada de aplicação)
  Challenge_Clyvo_NET.IntegrationTests/  Testes de integração (xUnit + WebApplicationFactory)
    Fixtures/                            ApiWebApplicationFactory + Collection Fixture
    Controllers/                         Testes ponta a ponta dos endpoints HTTP
```

Nomenclatura dos testes: `MetodoTestado_Cenario_ResultadoEsperado` (ex.:
`GetById_IdInexistente_RetornaNotFound`), e todo teste segue o padrão
**AAA** (Arrange, Act, Assert), com os três blocos comentados no corpo do método.

## Como rodar

```bash
dotnet test
# ou individualmente:
dotnet test tests/Challenge_Clyvo_NET.UnitTests
dotnet test tests/Challenge_Clyvo_NET.IntegrationTests
```

Nenhum dos dois projetos depende do Oracle real: os testes de integração
sobem a API com o provedor **EF Core InMemory** no lugar do Oracle (ver
`ApiWebApplicationFactory`), e os testes unitários mockam o `DbSet<T>`
diretamente com Moq/MockQueryable.

## Testes Unitários (`Challenge_Clyvo_NET.UnitTests`)

- **Domain**: testam construtores e o método `Update` de cada entidade
  (`Pessoa`, `Animal`, `Responsavel`, `Veterinario`, `Contato`, `Consulta`).
  São testes puros, sem mocks — as entidades não têm dependências externas.
- **Application**: testam o `PessoaController` mockando `AppDbContext.Pessoas`
  com **Moq** + **MockQueryable.Moq** (biblioteca que permite que `ToListAsync`,
  `FirstOrDefaultAsync`, etc. funcionem sobre uma lista em memória, sem tocar
  em banco). Cobre os fluxos de sucesso e de erro (404) de GET, POST, PUT e DELETE.

> **Por que só o `PessoaController` tem testes unitários com Moq?**
> Os demais controllers (`AnimaisController`, `ResponsavelController`,
> `VeterinarioController`, `ContatoController`, `ConsultaController`) usam
> `.Include()`/`.ThenInclude()` para eager loading dos relacionamentos.
> Bibliotecas de mock de `IQueryable` (MockQueryable, e mocks manuais de
> `DbSet<T>` em geral) não conseguem interpretar `.Include()`, porque ele
> depende do provedor real do EF Core para funcionar. Testar esses
> controllers "unitariamente" mockando o `DbSet` produziria um teste que
> não reflete o comportamento real (ou simplesmente quebraria em runtime).
> Por isso, a cobertura desses controllers fica a cargo dos **testes de
> integração** (que usam o provedor InMemory, um provedor real do EF Core,
> com suporte completo a `Include`). Se quiser Unit Tests com Moq para
> esses controllers também, o caminho recomendado é extrair uma camada de
> repositório/serviço com interface própria (ex.: `IAnimalRepository`) — hoje
> os controllers acessam o `AppDbContext` diretamente, sem essa abstração.

## Testes de Integração (`Challenge_Clyvo_NET.IntegrationTests`)

- `ApiWebApplicationFactory` (em `Fixtures/`): sobe a aplicação real via
  `WebApplicationFactory<Program>`, substituindo o registro do `AppDbContext`
  (Oracle) por um banco **InMemory** isolado por execução.
- `IntegrationTestCollection` (**Collection Fixture**, em `Fixtures/`): faz
  com que todas as classes de teste marcadas com `[Collection("Integration
  Tests")]` compartilhem a mesma instância da factory (mesmo servidor de
  teste, mesmo banco), em vez de recriar a aplicação inteira a cada classe.
- `PessoaEndpointsTests`: fluxo HTTP completo de `/api/pessoas` — criação,
  consulta, atualização, remoção e os respectivos casos de erro (404).
- `ObservabilityEndpointsTests`: valida o endpoint `/health` e o middleware
  de Correlation ID (geração automática e respeito ao header enviado pelo
  cliente).

> **Sobre autenticação:** a API atualmente **não implementa nenhum esquema
> de autenticação/autorização** (não há `[Authorize]`, JWT, API Key, etc. em
> nenhum controller). Por isso a suíte de integração não inclui testes desse
> tipo — não faria sentido simular uma autenticação que não existe no código.
> Se vocês adicionarem autenticação (ex.: JWT), o padrão a seguir é o mesmo
> usado aqui: um teste chamando o endpoint sem token (espera `401`), e outro
> com um token válido gerado no próprio teste (espera `200`/sucesso). Posso
> implementar isso assim que a autenticação existir na API.
