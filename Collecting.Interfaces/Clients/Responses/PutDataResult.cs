using System.Net;

namespace Collecting.Interfaces.Clients.Responses
{
    public class PutDataResult
    {
        public bool IsSuccess { get; private set; }

        public bool HasError { get => AnError != null; }

        public Exception AnError { get; private set; }

        public string Reason { get; private set; }

        public HttpStatusCode? StatusCode { get; private set; }

        public PutDataResult()
        {

        }
        protected virtual PutDataResult Assign(bool isSuccess, string reason, Exception error, HttpStatusCode? statusCode)
        {
            IsSuccess = isSuccess;
            AnError = error;
            StatusCode = statusCode;
            Reason = reason;

            return this;
        }

        public static T Success<T>() where T : PutDataResult, new()
        {
            var result = new T();
            result.Assign(true, null, null, HttpStatusCode.OK);
            return result;
        }

        public static T Success<T>(HttpStatusCode statusCode) where T : PutDataResult, new() 
        {
            var result = new T();
            result.Assign(true, null, null, statusCode);
            return result;
        }

        public static T Fail<T>(HttpStatusCode statusCode, string reason)
             where T : PutDataResult, new()
        {
            var result = new T();
            result.Assign(false, reason, null, statusCode);
            return result;
        }

        public static T Error<T>(Exception error) where T : PutDataResult, new()
        {
            var result = new T();
            result.Assign(false, null, error, null);
            return result;
        }
    }
}
