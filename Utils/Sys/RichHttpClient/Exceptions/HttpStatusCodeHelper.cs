using System.Net;

namespace Utils.Sys.Exceptions
{
    public static class HttpStatusCodeHelper
    {
        public const int OkCode = (int)HttpStatusCode.OK;
        public const int RedirectionCode = (int)HttpStatusCode.Ambiguous;
        public const int ServerErrorCode = (int)HttpStatusCode.InternalServerError;
        public const int RequestTimeoutErrorCode = (int)HttpStatusCode.RequestTimeout;

        public const int UnknownErrorCode = 520;

        public static bool IsSuccessStatusCode(HttpStatusCode statusCode)
        {
            return IsSuccessStatusCode((int)statusCode);
        }

        public static bool IsSuccessStatusCode(int statusCode)
        {
            return statusCode >= OkCode && statusCode < RedirectionCode;
        }

        public static bool IsValidationStatusCode(int statusCode)
        {
            return statusCode == (int)HttpStatusCode.NotFound || statusCode == (int)HttpStatusCode.BadRequest;
        }

        public static bool IsServerStatusCode(HttpStatusCode statusCode)
        {
            return IsServerStatusCode((int)statusCode);
        }

        public static bool IsServerStatusCode(int statusCode)
        {
            return statusCode >= ServerErrorCode;
        }
    }
}
