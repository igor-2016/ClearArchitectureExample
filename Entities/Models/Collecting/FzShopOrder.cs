using System.Xml.Serialization;

namespace Entities.Models.Collecting
{

    [XmlType("FzShopOrder")]
    public class FzShopOrder
    {
        [XmlElement(IsNullable = true)]
        public string? chequePrintDateTime { get; set; }

        [XmlElement(IsNullable = true)]
        public string? clientFullName { get; set; }

        [XmlElement(IsNullable = true)]
        public string? clientMobilePhone { get; set; }

        [XmlElement(IsNullable = true)]
        public string? clientMobilePhoneAlt1 { get; set; }

        [XmlElement(IsNullable = true)]
        public string? containerBarcodes { get; set; }

        [XmlElement(IsNullable = true)]
        public string? contragentFullName { get; set; }

        [XmlElement(IsNullable = true)]
        public string? contragentOKPO { get; set; }
        public string dateModified { get; set; }

        [XmlElement(IsNullable = true)]
        public string? deliveryAddress { get; set; }
        public string deliveryDate { get; set; }
        public int deliveryId { get; set; }
        public string deliveryTimeFrom { get; set; }
        public string deliveryTimeTo { get; set; }
        
        [XmlElement(IsNullable = true)]
        public string? driverId { get; set; }
        
        [XmlElement(IsNullable = true)]
        public string? driverName { get; set; }
        public int filialId { get; set; }

        [XmlElement(IsNullable = true)] 
        public string? globalUserId { get; set; }

        [XmlElement(IsNullable = true)] 
        public string? lastContainerBarcode { get; set; }

        [XmlElement(IsNullable = true)]
        public string? logisticsType { get; set; }

        [XmlElement(IsNullable = true)]
        public string? megaContainerBarcodes { get; set; }

        [XmlElement(IsNullable = true)]
        public string? orderBarcode { get; set; }
        public string orderCreated { get; set; }
        public string orderFrom { get; set; }

        [XmlElement("orderId")]
        public string orderId { get; set; }
        public string orderStatus { get; set; }
        public string paymentId { get; set; }
        public string placesCount { get; set; }
        public int priority { get; set; }

        [XmlElement(IsNullable = true)]
        public string? remark { get; set; }

        [XmlElement(IsNullable = true)]
        public string? rroNumber { get; set; }

        [XmlElement(IsNullable = true)]
        public string? sumPaymentFromInternet { get; set; }

        [XmlElement(IsNullable = true)]
        public string? sumPaymentFromKassa { get; set; }
    }
}
