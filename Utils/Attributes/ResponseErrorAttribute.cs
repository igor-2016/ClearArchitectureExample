using System.Net;

namespace Utils.Attributes
{
    public class ResponseErrorAttribute : Attribute
    {
        public string Message { get; set; }
        public HttpStatusCode HttpStatus { get; set; }
    }
}