using Entities.Models.Expansion;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Interfaces.Dto
{
   
    public class TraceableOrderDto : RowVersionEntityDto, IHasBasket
    {

        public TraceableOrderDto()
        {
            Items = new List<TraceableOrderItemDto>();
        }

        [Column("basketId")]
        public Guid BasketId { get; set; }

        [Column("orderNumber", TypeName = "nvarchar(64)")]
        public string OrderNumber { get; set; } 

        [Column("externalOrderId")]
        public int? ExternalOrderId { get; set; } // in fz db int !!! у Чернова


        [Column("chequePrintDateTime", TypeName = "datetime")]
        public DateTime? ChequePrintDateTime { get; set; }


        [Column("clientFullName", TypeName = "nvarchar(98)")]
        public string ClientFullName { get; set; }


        [Column("clientMobilePhone", TypeName = "nvarchar(13)")]
        public string ClientMobilePhone { get; set; }


        [Column("clientMobilePhoneAlt1", TypeName = "nvarchar(13)")]
        public string ClientMobilePhoneAlt1 { get; set; }

        // max
        [Column("containerBarcodes", TypeName = "nvarchar(128)")]
        public string ContainerBarcodes { get; set; }


        [Column("contragentFullName", TypeName = "nvarchar(255)")]
        public string ContragentFullName { get; set; }


        [Column("contragentOKPO", TypeName = "nvarchar(20)")]
        public string ContragentOKPO { get; set; }

        
        [Column("dateModified", TypeName = "datetime")]
        public DateTime DateModified { get; set; }


        [Column("deliveryAddress", TypeName = "nvarchar(250)")]
        public string DeliveryAddress { get; set; }


        [Column("deliveryDate", TypeName = "datetime")] 
        public DateTime DeliveryDate { get; set; }


        [Column("deliveryId")]
        public int DeliveryId { get; set; } 


        [Column("deliveryTimeFrom", TypeName = "time(0)")]
        public TimeSpan DeliveryTimeFrom { get; set; }


        [Column("deliveryTimeTo", TypeName = "time(0)")]
        public TimeSpan DeliveryTimeTo { get; set; }


        [Column("driverId")]
        public int? DriverId { get; set; } // not in db


        [Column("driverName", TypeName = "nvarchar(98)")]
        public string DriverName { get; set; }


        [Column("filialId")]
        public int FilialId { get; set; }


        [Column("globalUserId")]
        public int? GlobalUserId { get; set; }

        [Column("pickerName", TypeName = "nvarchar(98)")]
        public string PickerName { get; set; }

        [Column("userInn", TypeName = "nvarchar(20)")]
        public string UserInn { get; set; } // added not in fz db




        [Column("lastContainerBarcode", TypeName = "nvarchar(128)")]
        public string LastContainerBarcode { get; set; }

        
        [Column("logisticsType", TypeName = "nvarchar(100)")]
        public string LogisticsType { get; set; }


        [Column("megaContainerBarcodes", TypeName = "nvarchar(1000)")]
        public string MegaContainerBarcodes { get; set; } // not in fz db


        [Column("orderBarcode", TypeName = "nvarchar(40)")]
        public string OrderBarcode { get; set; }


        [Column("orderCreated", TypeName = "datetime")]
        public DateTime OrderCreated { get; set; }

        
        [Column("orderFrom", TypeName = "tinyint")]
        public int OrderFrom { get; set; } 


       


        [Column("orderStatus")]
        public int OrderStatus { get; set; }


        [Column("paymentId")]
        public int PaymentId { get; set; }


        [Column("placesCount")]
        public int? PlacesCount { get; set; }


        [Column("priority")]
        public int Priority { get; set; }


        [Column("remark", TypeName = "nvarchar(250)")]
        public string Remark { get; set; }


        [Column("rroNumber", TypeName = "nvarchar(25)")]
        public string RroNumber { get; set; }


        [Column("sumPaymentFromInternet", TypeName = "decimal(18,5)")]
        public decimal? SumPaymentFromInternet { get; set; }


        [Column("sumPaymentFromKassa", TypeName = "decimal(18,5)")]
        public decimal? SumPaymentFromKassa { get; set; }

        [Column("collectingState")]
        public int CollectingState { get; set; }

        [Column("collectStartTime", TypeName = "datetime")]
        public DateTime? CollectStartTime { get; set; }

        [Column("collectEndTime", TypeName = "datetime")]
        public DateTime? CollectEndTime { get; set; }

        [Column("merchantId")]
        public int MerchantId { get; set; }

        public List<TraceableOrderItemDto> Items { get; set; }
    }
}
