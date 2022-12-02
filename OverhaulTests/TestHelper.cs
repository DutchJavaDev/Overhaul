using Microsoft.Extensions.Configuration;

namespace OverhaulTests
{
    public static class TestHelper
    {
        private readonly static Dictionary<string, string> _cache = new();

        public static string? GetString(string section)
        {
            if(_cache.ContainsKey(section))
            {
                return _cache[section];
            }

            var builder = new ConfigurationBuilder()
               .AddEnvironmentVariables()
               .AddUserSecrets(typeof(TestHelper).Assembly);
            // Order is reversed when looking up keys

            var configurationRoot = builder.Build();

            if (configurationRoot[section] != null)
            { 
                _cache[section] = configurationRoot[section];

                return _cache[section];
            }

            var environment = Environment.GetEnvironmentVariables(EnvironmentVariableTarget.Machine);

            if (environment.Contains(section.ToUpper()))
            {
                _cache.Add(section, (string?)environment[section.ToUpper()] ?? "");
                return _cache[section];
            }

            return string.Empty;
        }
    }
}