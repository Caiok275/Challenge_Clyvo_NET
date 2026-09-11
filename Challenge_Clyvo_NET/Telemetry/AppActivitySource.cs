using System.Diagnostics;

namespace Challenge_Clyvo_NET.Telemetry
{
    /// <summary>
    /// Fonte de Activities (spans) customizada da aplicação. As instrumentações
    /// automáticas (ASP.NET Core, EF Core, HttpClient) já cobrem a maior parte
    /// do rastreamento entre camadas; esta fonte existe para criar spans manuais
    /// em pontos de negócio específicos que valham a pena destacar em um trace
    /// (ex.: uma regra de validação cara, uma orquestração entre várias
    /// entidades, etc.), usando: 
    ///   using var activity = AppActivitySource.Instance.StartActivity("NomeDaOperacao");
    ///   activity?.SetTag("chave", valor);
    /// </summary>
    public static class AppActivitySource
    {
        public const string Name = "Challenge_Clyvo_NET";
        public const string Version = "1.0.0";

        public static readonly ActivitySource Instance = new(Name, Version);
    }
}
