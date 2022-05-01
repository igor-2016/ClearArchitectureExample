using Microsoft.AspNetCore.Cors.Infrastructure;

namespace ECom.Expansion.Service.Utils
{
    public class CorsPolicySetting : CorsPolicy
    {
        public string Name { get; set; }
    }
}
