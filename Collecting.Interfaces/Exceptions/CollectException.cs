using Entities;
using System.Net;
using System.Runtime.Serialization;
using Utils.Attributes;
using Utils.Consts;

namespace Collecting.Interfaces
{
    [Serializable]
    public class CollectException : DescribedException
    {
        public override string ExceptionSource { get; protected set; } = CommonConsts.Project.CollectingService;


        public CollectException(CollectErrors error) :
            base((int)error, error.GetResponseMessageFromEnum(), error.GetHttpStatusFromEnum())
        {
        }

        public CollectException(CollectErrors error, Exception ex) :
            base((int)error, error.GetResponseMessageFromEnum(),  error.GetHttpStatusFromEnum(), ex)
        {
        }

        public CollectException(CollectErrors error, HttpStatusCode httpStatusCode, Exception ex) :
            base((int)error, error.GetResponseMessageFromEnum(), httpStatusCode, ex)
        {
        }

        public CollectException(CollectErrors error, string description, Exception ex) : 
            base((int)error, error.GetResponseMessageFromEnum(), description, error.GetHttpStatusFromEnum(), ex)
        {
        }

        public CollectException(CollectErrors error, string description, HttpStatusCode httpStatusCode, Exception ex) :
           base((int)error, error.GetResponseMessageFromEnum(), description, httpStatusCode, ex)
        {
        }

        public CollectException(CollectErrors error, string description) : 
            base((int)error, error.GetResponseMessageFromEnum(), description, error.GetHttpStatusFromEnum())
        {
        }
        public CollectException(CollectErrors error, string description, HttpStatusCode httpStatusCode) : 
            base((int)error, error.GetResponseMessageFromEnum(), description, httpStatusCode)
        {
        }

        protected CollectException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {

        }

        

    }
}
