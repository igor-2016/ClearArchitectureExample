using System.Xml.Serialization;

namespace WebSiteService.Interfaces.Clients.Responses
{ 

    [XmlRoot("ConfirmResponse")]
    public class FozzyClientConfirmResponse
    {
        public const string OK = "0";
        public string errorCode { get; set; }
        public string errorMessage { get; set; }
    }
}
