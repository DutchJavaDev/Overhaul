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

            var environment = Environment.GetEnvironmentVariables(EnvironmentVariableTarget.Machine);

            if (environment.Contains(section.ToUpper()))
            {
                _cache.Add(section, (string)environment[section.ToUpper()]);
                return _cache[section];
            }

            // Could put path i enviroment variables
            // Researc
             var builder = new ConfigurationBuilder()
                .AddUserSecrets(typeof(TestHelper).Assembly)
                .AddEnvironmentVariables();

            var configurationRoot = builder.Build();

            _cache[section] = configurationRoot[section];

            return _cache[section];
        }
    }
}