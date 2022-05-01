using Microsoft.Extensions.Logging;

namespace Utils
{
    public static class LoggerExtensions
    {
        public static IDisposable LoggingScope(this ILogger logger, Guid basketGuid, int userId = 0)
        {
            return logger.BeginScope(new Dictionary<string, object>
            {
                ["basketGuid"] = basketGuid,
                ["userId"] = userId
            });
        }


      
    }
}
