using Entities;
using System.Net;
using System.Runtime.Serialization;
using Utils.Attributes;
using Utils.Consts;

namespace Catalog.Interfaces.Exceptions
{
    public class CatalogException : DescribedException
    {
        public override string ExceptionSource { get; protected set; } = CommonConsts.Project.CatalogService;


        public CatalogException(CatalogErrors error) :
            base((int)error, error.GetResponseMessageFromEnum(), error.GetHttpStatusFromEnum())
        {
        }

        public CatalogException(CatalogErrors error, string errorMessage, string errorDescription) :
            base((int)error, errorMessage, errorDescription, error.GetHttpStatusFromEnum())
        {
        }

        public CatalogException(CatalogErrors error, string errorMessage) :
          base((int)error, error.GetResponseMessageFromEnum(), errorMessage, error.GetHttpStatusFromEnum())
        {
        }

        public CatalogException(CatalogErrors error, Exception ex) :
            base((int)error, error.GetResponseMessageFromEnum(), error.GetHttpStatusFromEnum(), ex)
        {
        }

        public CatalogException(CatalogErrors error, HttpStatusCode httpStatusCode, Exception ex) :
            base((int)error, error.GetResponseMessageFromEnum(), httpStatusCode, ex)
        {
        }

        public CatalogException(CatalogErrors error, string description, Exception ex) :
            base((int)error, error.GetResponseMessageFromEnum(), description, error.GetHttpStatusFromEnum(), ex)
        {
        }

        protected CatalogException(SerializationInfo serializationInfo, StreamingContext streamingContext)
           : base(serializationInfo, streamingContext)
        {

        }
    }
}
