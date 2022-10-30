using Microsoft.Extensions.Configuration;

namespace OverhaulTests
{
    internal static class TestHelper
    {
        private static string? _cache;

        public static string GetConnectionString()
        {
            if(!string.IsNullOrEmpty(_cache))
            {
                return _cache;
            }

            var environment = Environment.GetEnvironmentVariable("NETCORE_ENVIRONMENT");

            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{environment}.json", optional: true)
                .AddEnvironmentVariables();

            var configurationRoot = builder.Build();

            
            _cache = configurationRoot["devString"];

            return _cache;
        }
    }
}