using System.Net;
using System.Net.Http;

namespace Utils.Sys.RichHttpClient.Exceptions
{
    internal class HttpServerErrorException : RichHttpClientException
    {
        
        internal HttpServerErrorException(HttpStatusCode code, string message, Exception ex)
            : base(code, message, ex)
        {
        }

        internal HttpServerErrorException(HttpStatusCode code, Exception ex)
            : base(code, code.ToString(), ex)
        {
        }
    }
}
