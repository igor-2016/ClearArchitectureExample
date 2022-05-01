using ApplicationServices.Interfaces.Enums;
using Collecting.Interfaces.Enums;
using ECom.Types.Orders;

namespace ApplicationServices.Interfaces.Models
{

    public class TraceableOrderView : VersionedEntityView<Guid>
    {
        public TraceableOrderView()
        {
            Items = new List<TraceableOrderItemView>();
        }

        public Guid BasketId { get; set; }

        public string OrderNumber { get; set; } 

        public int? ExternalOrderId { get; set; } // in fz db int !!! у Чернова


        public DateTime? ChequePrintDateTime { get; set; }


        public string ClientFullName { get; set; }


        public string ClientMobilePhone { get; set; }


        public string ClientMobilePhoneAlt1 { get; set; }

        // max
        public string ContainerBarcodes { get; set; }


        public string ContragentFullName { get; set; }


        public string ContragentOKPO { get; set; }

        
        public DateTime DateModified { get; set; }


        public string DeliveryAddress { get; set; }


        public DateTime DeliveryDate { get; set; }

        /// <summary>
        /// Fozzy DeliveryType
        /// </summary>
        public FozzyDeliveryType DeliveryId { get; set; } 


        public TimeSpan DeliveryTimeFrom { get; set; }


        public TimeSpan DeliveryTimeTo { get; set; }


        public DriverView Driver { get; set; } 

        public int FilialId { get; set; }


        public string LastContainerBarcode { get; set; }

        
        public string LogisticsType { get; set; }


        public string MegaContainerBarcodes { get; set; } // not in fz db


        public string OrderBarcode { get; set; }


        public DateTime OrderCreated { get; set; }

        
        public FozzyOrderOrigin OrderFrom { get; set; } 

        /// <summary>
        /// Состояние ордера
        /// </summary>
        public int OrderStatus { get; set; }


        public FozzyPaymentType PaymentId { get; set; }


        public int? PlacesCount { get; set; }


        public int Priority { get; set; }


        public string Remark { get; set; }


        public string RroNumber { get; set; }


        public decimal? SumPaymentFromInternet { get; set; }


        public decimal? SumPaymentFromKassa { get; set; }

       
        public OrderCollectingState CollectingState { get; set; }
        
        public DateTime? CollectStartTime { get; set; }

        public DateTime? CollectEndTime { get; set; }


        public Merchant MerchantId { get; set; }

        public IList<TraceableOrderItemView> Items { get; set; }

        public PickerView Picker { get; set; }
        
    }
}
