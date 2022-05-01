using System.Net;

namespace Collecting.Interfaces.Clients.Responses
{
    public class DataResultBase
    {
        public bool IsSuccess { get; }

        public bool HasError { get => AnError != null; }

        public Exception AnError { get; }

        public string Reason { get; }

        public HttpStatusCode? StatusCode { get; }

        protected DataResultBase(bool isSuccess, string reason, Exception error, HttpStatusCode? statusCode)
        {
            IsSuccess = isSuccess;
            AnError = error;
            StatusCode = statusCode;
            Reason = reason;
        }
    }
}
