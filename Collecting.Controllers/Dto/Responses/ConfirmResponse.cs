using Collecting.Interfaces;
using Collecting.Interfaces.Exceptions;
using DomainServices.Interfaces;
using Utils.Attributes;

namespace Collecting.Controllers.Responses
{
    public class ConfirmResponse : IErrorCodeResponse
    {
        public int errorCode { get; set; }

        public string errorMessage { get; set; }

        public static ConfirmResponse OK() => new ConfirmResponse();

        public static ConfirmResponse Error(int errorId, string message) => 
            new ConfirmResponse() 
            { 
                errorCode = errorId, 
                errorMessage = message 
            };

        public static ConfirmResponse Error(CollectErrors error) =>
            new ConfirmResponse()
            {
                errorCode = error.GetHttpStatusFromEnum().ToInt(),
                errorMessage = error.GetResponseMessageFromEnum()
            };

        public static ConfirmResponse NotFound() => 
            new ConfirmResponse()
            {
                errorCode = (int)CollectErrors.OrderNotFound,
                errorMessage = CollectErrors.OrderNotFound.GetResponseMessageFromEnum()
            };

    }
}
