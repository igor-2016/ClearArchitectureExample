using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ApplicationServices.Interfaces.Extensions
{
    public static class LoggingExtensions
    {
        public static void LogCustomError(this ILogger logger, Exception ex, HttpContext httpContext= null)
        {
            object requestGuid = null;
            httpContext?.Items.TryGetValue("requestGuid", out requestGuid);

            if (ex is DescribedException describedException)
            {
                logger.Log(LogLevel.Warning, "RequestGuid: {requestGuid}, ErrorId: {errorId}, ErrorCode: {errorCode}, ErrorMessage: {errorMessage}, ErrorDesc: {errorDescription}, ErrorSource: {errorSource}",
                       requestGuid?.ToString(),
                       describedException.ErrorId,
                       describedException.ErrorCode,
                       describedException.ErrorMessage,
                       describedException.ErrorDescription,
                       describedException.ServiceSource);
            }
            else
            {
                string? stackTrace = ex?.StackTrace;
                logger.Log(LogLevel.Error, "RequestGuid: {requestGuid}, ErrorMessage: {errorMessage}, StackTrace: {stackTrace}",
                       requestGuid?.ToString(),
                       ex?.Message,
                       stackTrace);
            }
        }
    }
}
