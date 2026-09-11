using System.Diagnostics;

namespace Challenge_Clyvo_NET.Telemetry
{
    public static class AppActivitySource
    {
        public const string Name = "Challenge_Clyvo_NET";
        public const string Version = "1.0.0";

        public static readonly ActivitySource Instance = new(Name, Version);
    }
}
