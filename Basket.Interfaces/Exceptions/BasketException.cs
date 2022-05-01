using ECom.Types.Exceptions;
using Entities;
using System.Net;
using System.Runtime.Serialization;
using Utils.Attributes;
using Utils.Consts;

namespace Basket.Interfaces
{
    [Serializable]
    public class BasketException : DescribedException, IHttpError
    {
        public override string ExceptionSource { get; protected set; } = CommonConsts.Project.BasketService;

        public BasketException(BasketErrors error) : base((int)error, error.GetResponseMessageFromEnum())
        {
            HttpStatus = error.GetHttpStatusFromEnum();
        }

        public BasketException(BasketErrors error, string description) : 
            base((int)error, error.GetResponseMessageFromEnum(), description)
        {
            HttpStatus = error.GetHttpStatusFromEnum();
        }

        public BasketException(BasketErrors error, string errorMessage, string description) :
            base((int)error, errorMessage, description)
        {
            HttpStatus = error.GetHttpStatusFromEnum();
        }


        public BasketException(BasketErrors error, string description, Exception ex) :
            base((int)error, error.GetResponseMessageFromEnum(), description, ex)
        {
            HttpStatus = error.GetHttpStatusFromEnum();
        }
        protected BasketException(SerializationInfo serializationInfo, StreamingContext streamingContext)
           : base(serializationInfo, streamingContext)
        {

        }

        #region IHttpError
        public HttpStatusCode HttpStatus { get; }

        public ErrorCodeResponse ResponseMessage()
        {
            return new ErrorCodeResponse
            {
                ErrorCode = ErrorCode,
                ErrorMessage = ErrorMessage,
                ErrorDescription = Message
            };
        }

       
        #endregion

    }
}
