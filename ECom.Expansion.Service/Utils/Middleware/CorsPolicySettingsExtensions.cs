namespace ECom.Expansion.Service.Utils
{
    public static class CorsPolicySettingsExtensions
    {
        public static void AddCorsPolicies(this IServiceCollection serviceCollection, IConfiguration configurationRoot)
        {
            serviceCollection.AddCorsPolicies(configurationRoot.GetSection("CORSPolicies"));
        }

        public static void AddCorsPolicies(this IServiceCollection serviceCollection, IConfigurationSection configurationSection)
        {
            var corsPolicies = configurationSection.Get<IEnumerable<CorsPolicySetting>>();

            serviceCollection.AddCors(builder =>
            {
                foreach (var corsPolicy in corsPolicies)
                {
                    builder.AddPolicy(corsPolicy.Name, corsPolicy);
                }
            });
        }
    }
}
