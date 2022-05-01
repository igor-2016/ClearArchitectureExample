using System.Net;

namespace ApplicationServices.Interfaces.Responses
{
    public class DataResult<T>
    {
        public bool IsSuccess { get; }

        public bool HasError { get => AnError != null; }

        public Exception AnError { get; }

        public T Data { get; }

        public string Reason { get; }

        public HttpStatusCode? StatusCode { get; }

        protected DataResult(bool isSuccess, T data, string reason, Exception error, HttpStatusCode? statusCode)
        {
            IsSuccess = isSuccess;
            AnError = error;
            StatusCode = statusCode;
            Reason = reason;
            Data = data;
        }

        public static DataResult<T> Success(T data, HttpStatusCode statusCode = HttpStatusCode.OK) =>
            new DataResult<T>(true, data, null, null, statusCode);

        public static DataResult<T> Fail(HttpStatusCode statusCode, string reason) =>
           new DataResult<T>(false, default, reason, new Exception(reason), statusCode);

        public static DataResult<T> Error(Exception error) =>
            new DataResult<T>(false, default, null, error, null);

        public static DataResult<T> NotFound(string error) =>
             new DataResult<T>(false, default, error, null, HttpStatusCode.NotFound);

    }
}
