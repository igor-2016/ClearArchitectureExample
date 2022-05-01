using System.Net;

namespace Workflow.Interfaces.Clients.Responses
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
        }

        public static DataResult<T> Success(T data, HttpStatusCode statusCode) =>
            new DataResult<T>(true, data, null, null, statusCode);

        public static DataResult<T> Fail(HttpStatusCode statusCode, string reason) =>
           new DataResult<T>(false, default, reason, new Exception(reason), statusCode);

        public static DataResult<T> Error(Exception error) =>
            new DataResult<T>(false, default, null, error, null);

        public static DataResult<T> NotFound(string error) =>
             new DataResult<T>(false, default, null, new Exception(error)
                 , HttpStatusCode.NotFound);

    }
    /*
    public class DataResult<T>
    {
        public bool IsSuccess { get; }

        public bool HasError { get => AnError != null; }

        public Exception? AnError { get; }

        public T? Data { get; }

        public string? Reason { get; }

   
        protected DataResult(bool isSuccess, T? data, string? reason, Exception? error)
        {
            IsSuccess = isSuccess;
            AnError = error;
            Reason = reason;
            Data = data;
        }

        public static DataResult<T> Success(T data) =>
            new(true, data, null, null);

        public static DataResult<T> Fail(string reason) =>
           new(false, default, reason, new Exception(reason));

        public static DataResult<T> Error(Exception error) =>
            new(false, default, null, error);

        public static DataResult<T> NotFound(string error) =>
             new(false, default, null, new Exception(error));

    }
    */
}
