using Entities;
using System.Net;
using System.Runtime.Serialization;
using Utils.Attributes;
using Utils.Consts;

namespace WebSiteService.Interfaces
{
    [Serializable]
    public class SiteException : DescribedException
    {
        public override string ExceptionSource { get; protected set; } = CommonConsts.Project.WebSite;

        public SiteException(SiteErrors error, Exception ex) :
            base((int)error, error.GetResponseMessageFromEnum(),  error.GetHttpStatusFromEnum(), ex)
        {
        }

        public SiteException(SiteErrors error, HttpStatusCode httpStatusCode, Exception ex) :
            base((int)error, error.GetResponseMessageFromEnum(), httpStatusCode, ex)
        {
        }

        public SiteException(SiteErrors error, string description, Exception ex) : 
            base((int)error, error.GetResponseMessageFromEnum(), description, error.GetHttpStatusFromEnum(), ex)
        {
        }

        public SiteException(SiteErrors error, string description, HttpStatusCode httpStatusCode, Exception ex) :
           base((int)error, error.GetResponseMessageFromEnum(), description, httpStatusCode, ex)
        {
        }

        public SiteException(SiteErrors error, string description) : 
            base((int)error, error.GetResponseMessageFromEnum(), description, error.GetHttpStatusFromEnum())
        {
        }
        public SiteException(SiteErrors error, string description, HttpStatusCode httpStatusCode) : 
            base((int)error, error.GetResponseMessageFromEnum(), description, httpStatusCode)
        {
        }

        protected SiteException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {

        }

       

    }
}
