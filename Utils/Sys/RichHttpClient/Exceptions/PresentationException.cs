using System.Net;

namespace Utils.Sys.Exceptions
{
    public class PresentationException<TErrorModel> : Exception
        where TErrorModel : class
    {
        public PresentationException(HttpStatusCode statusCode, TErrorModel errorModel) : base(errorModel.ToJson())
        {
            StatusCode = statusCode;
            ErrorModel = errorModel;
        }

        public TErrorModel ErrorModel { get; set; } 

        public HttpStatusCode StatusCode { get; set; }
    }
}
