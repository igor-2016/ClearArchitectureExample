namespace ApplicationServices.Interfaces
{
    public class CheckDataResult<T>
    {
        public bool IsSuccess { get; }

        public bool HasError { get => AnError != null; }

        public Exception? AnError { get; }

        public T? Data { get; }

        public string? Reason { get; }


        protected CheckDataResult(bool isSuccess, T? data, string? reason, Exception? error)
        {
            IsSuccess = isSuccess;
            AnError = error;
            Reason = reason;
            Data = data;
        }

        public static CheckDataResult<T> Success(T data) =>
            new(true, data, null, null);

        public static CheckDataResult<T> Fail(string reason) =>
           new(false, default, reason, new Exception(reason));

        public static CheckDataResult<T> Error(Exception error) =>
            new(false, default, null, error);

        public static CheckDataResult<T> NotFound(string error) =>
             new(false, default, null, new Exception(error));

    }
}
