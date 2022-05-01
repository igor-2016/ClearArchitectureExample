
using System.Xml.Serialization;

namespace Collecting.Interfaces.Clients.Responses
{
    [XmlType("OrderData")]
    public class FozzyOrderData
    {
        [XmlArray("order")]
        public FozzyOrder[] order { get; set; }

        [XmlArray("orderLines")]
        public FozzyOrderLines[] orderLines { get; set; }
    }
}
