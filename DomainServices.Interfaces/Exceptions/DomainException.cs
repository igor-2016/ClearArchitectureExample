using Entities;
using System.Runtime.Serialization;
using Utils.Attributes;
using Utils.Consts;

namespace DomainServices.Interfaces.Exceptions
{
    [Serializable]
    public class DomainException : DescribedException
    {
        public override string ExceptionSource { get; protected set; } = CommonConsts.Project.Entities;

        public DomainException(DomainErrors error, string description, Exception ex) :
            base((int)error, error.GetResponseMessageFromEnum(), description, error.GetHttpStatusFromEnum(), ex)
        {
        }

        public DomainException(DomainErrors error, string description) :
            base((int)error, error.GetResponseMessageFromEnum(), description, error.GetHttpStatusFromEnum())
        {
        }


        protected DomainException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {

        }
    }
}
