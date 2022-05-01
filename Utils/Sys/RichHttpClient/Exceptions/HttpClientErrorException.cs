using System.Net;

namespace Utils.Sys.RichHttpClient.Exceptions
{
    internal class HttpClientErrorException : RichHttpClientException
    {
        internal HttpClientErrorException(HttpStatusCode code, string message, Exception ex)
            : base(code, message, ex)
        {
        }

        internal HttpClientErrorException(HttpStatusCode code, Exception ex)
            : base(code, code.ToString(), ex)
        {
        }
    }
}
