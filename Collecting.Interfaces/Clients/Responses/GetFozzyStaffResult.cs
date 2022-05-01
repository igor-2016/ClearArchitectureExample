using System.Net;

namespace Collecting.Interfaces.Clients.Responses
{
    public class GetFozzyStaffResult : DataResultBase
    {
        public FozzyStaff Staff { get; }

        public bool IsNotFound => StatusCode == HttpStatusCode.NotFound;

        protected GetFozzyStaffResult
            (bool isSuccess, FozzyStaff staff, string reason, Exception error, HttpStatusCode? statusCode)
            : base(isSuccess, reason, error, statusCode)
        {
            Staff = staff;
        }

        public static GetFozzyStaffResult Success(FozzyStaff staff) =>
           new GetFozzyStaffResult(true, staff, null, null, HttpStatusCode.OK);

        public static GetFozzyStaffResult Success(FozzyStaff staff, HttpStatusCode statusCode) =>
            new GetFozzyStaffResult(true, staff, null, null, statusCode);

        public static GetFozzyStaffResult Fail(HttpStatusCode statusCode, string reason) =>
           new GetFozzyStaffResult(false, null, reason, new Exception(reason), statusCode);

        public static GetFozzyStaffResult ErrorPublic(string errorMessage, CollectErrors error, HttpStatusCode statusCode) =>
           new GetFozzyStaffResult(false, null, null, new CollectException(error, errorMessage, statusCode), statusCode);

        public static GetFozzyStaffResult ErrorPublic(string errorMessage, CollectErrors error, HttpStatusCode statusCode, Exception ex) =>
           new GetFozzyStaffResult(false, null, null, new CollectException(error, errorMessage, statusCode, ex), statusCode);

        public static GetFozzyStaffResult Error(Exception error) =>
            new GetFozzyStaffResult(false, null, null, error, null);

        public static GetFozzyStaffResult Error(CollectErrors error, Exception ex) =>
            new GetFozzyStaffResult(false, null, null, new CollectException(error, ex), null);

        public static GetFozzyStaffResult Error(CollectErrors error, HttpStatusCode httpStatusCode, Exception ex) =>
           new GetFozzyStaffResult(false, null, null, new CollectException(error, httpStatusCode, ex), httpStatusCode);

        public static GetFozzyStaffResult Error(CollectErrors error, HttpStatusCode httpStatusCode, string description, Exception ex) =>
           new GetFozzyStaffResult(false, null, description, new CollectException(error, description, httpStatusCode, ex), httpStatusCode);


        public static GetFozzyStaffResult NotFound(string errorMessage, CollectErrors error) =>
           new GetFozzyStaffResult(false, null, null, new CollectException(error, errorMessage, HttpStatusCode.NotFound), HttpStatusCode.NotFound);

    //    public static FozzyShopGetStaffResult NotFound(string error) =>
    //        new FozzyShopGetStaffResult(false, null, null, new CollectException(CollectErrors.GlobalUserIdInnMappingNotFound, error), HttpStatusCode.NotFound);
    }
}
