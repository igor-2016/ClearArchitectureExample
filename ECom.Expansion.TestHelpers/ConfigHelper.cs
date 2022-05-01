using Microsoft.Extensions.Configuration;

namespace ECom.Expansion.TestHelpers
{
    public static class ConfigHelper
    {
        public static IConfiguration InitConfiguration()
        {
            var config = new ConfigurationBuilder()
               .AddJsonFile("appsettings.test.json")
               .AddEnvironmentVariables()
               .Build();
            return config;
        }
    }
}
