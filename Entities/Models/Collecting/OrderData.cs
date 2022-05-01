using System.Xml.Serialization;

namespace Entities.Models.Collecting
{
    [XmlType("OrderData")]
    public class OrderData
    {
        [XmlArray("order")]
        public FzShopOrder[] order { get; set; }

        [XmlArray("orderLines")]
        public FzShopOrderLines[] orderLines { get; set; }
    }
}
