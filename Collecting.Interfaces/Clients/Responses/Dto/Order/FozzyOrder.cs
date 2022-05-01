using System.Xml.Serialization;

namespace Collecting.Interfaces.Clients.Responses
{
    [XmlType("FzShopOrder")]
    public class FozzyOrder
    {
        public string chequePrintDateTime { get; set; }

        public string clientFullName { get; set; }

        public string clientMobilePhone { get; set; }

        public string clientMobilePhoneAlt1 { get; set; }

        public string containerBarcodes { get; set; }

        public string contragentFullName { get; set; }

        public string contragentOKPO { get; set; }
        public string dateModified { get; set; }

        public string deliveryAddress { get; set; }
        public string deliveryDate { get; set; }
        public int deliveryId { get; set; }
        public string deliveryTimeFrom { get; set; }
        public string deliveryTimeTo { get; set; }
        
        public string driverId { get; set; }
        
        public string driverName { get; set; }
        public int filialId { get; set; }

        public string globalUserId { get; set; }

        public string lastContainerBarcode { get; set; }

        public string logisticsType { get; set; }

        public string megaContainerBarcodes { get; set; }

        public string orderBarcode { get; set; }
        public string orderCreated { get; set; }
        public string orderFrom { get; set; }

        [XmlElement("orderId")]
        public string orderId { get; set; }
        
        [XmlElement("orderStatus")]
        public string orderStatus { get; set; }
        public string paymentId { get; set; }
        public string placesCount { get; set; }
        public int priority { get; set; }

        public string remark { get; set; }

        public string rroNumber { get; set; }

        public string sumPaymentFromInternet { get; set; }

        public string sumPaymentFromKassa { get; set; }
    }
}
