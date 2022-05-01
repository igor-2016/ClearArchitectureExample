using System.Net;
using System.Text;

namespace Utils.Sys.RichHttpClient.Exceptions
{
    public class RichHttpClientException : Exception
    {
        public HttpStatusCode? Code { get; }

        public string? Response { get; }

        public RichHttpClientException(HttpStatusCode? code, string? stringResponse) : base(stringResponse)
        {
            Code = code;
            Response = stringResponse;
        }

        public RichHttpClientException(HttpStatusCode? code, string message, string? stringResponse, Exception inner)
            : base(message, inner)
        {
            Code = code;
            Response = stringResponse;
        }

        public RichHttpClientException(HttpStatusCode? code, string? stringResponse, Exception inner)
            : base(inner.Message, inner)
        {
            Code = code;
            Response = stringResponse;
        }

        public override string ToString()
        {
            var str = new StringBuilder();

            if (!string.IsNullOrEmpty(Message))
            {
                str.AppendLine($"{GetType()}: {Message}");
            }
            else
            {
                str.AppendLine(GetType().ToString());
            }

            str.AppendLine($"HTTP code: {Code}");

            if (!string.IsNullOrEmpty(Response))
            {
                str.AppendLine($"Response: {Response}");
            }

            if (InnerException != null)
            {
                str.AppendLine($" ---> {InnerException}");
            }

            var stackTrace = StackTrace;
            if (stackTrace != null)
            {
                str.AppendLine(stackTrace);
            }

            return str.ToString();
        }
    }

}
