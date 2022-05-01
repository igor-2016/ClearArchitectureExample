using System.Net;
using Utils.Sys.RichHttpClient.Exceptions;

namespace Utils.Sys.Exceptions
{
    public class NotFoundException : RichHttpClientException
    {
        public NotFoundException(string content, Exception ex) 
            : base(HttpStatusCode.NotFound, content, ex)
        {
        }
    }
}
