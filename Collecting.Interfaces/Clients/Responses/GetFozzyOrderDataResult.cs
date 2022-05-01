using Entities.Models.Collecting;
using System.Net;

namespace Collecting.Interfaces.Clients.Responses
{

    public class GetFozzyOrderDataResult : DataResultBase
    {
        public FozzyOrderData Order { get; }

        protected GetFozzyOrderDataResult
            (bool isSuccess, FozzyOrderData order, string reason, Exception error, HttpStatusCode? statusCode)
            : base (isSuccess, reason, error, statusCode)
        {
            Order = order;
        }

        public static GetFozzyOrderDataResult Success(FozzyOrderData order) =>
            new GetFozzyOrderDataResult(true, order, null, null, HttpStatusCode.OK);
        public static GetFozzyOrderDataResult Success(FozzyOrderData order, HttpStatusCode statusCode) =>
            new GetFozzyOrderDataResult(true, order, null, null, statusCode);

        public static GetFozzyOrderDataResult Fail(HttpStatusCode statusCode, string reason) =>
           new GetFozzyOrderDataResult(false, null, reason, new Exception(reason), statusCode);

        public static GetFozzyOrderDataResult Error(Exception error) =>
            new GetFozzyOrderDataResult(false, null, null, error, null);


    }
}
