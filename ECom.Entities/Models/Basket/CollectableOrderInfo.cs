using ECom.Types.Delivery;
using ECom.Types.Orders;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace ECom.Entities.Models
{

    public class FozzyCollectableOrderInfo 
    {
        [JsonProperty("basketGuid")]
        public Guid BasketGuid { get; set; }

        [JsonProperty("orderNumber")]
        public string OrderNumber { get; set; }

        [JsonProperty("orderBarCode", Required = Required.AllowNull)]
        public string? OrderBarCode { get; set; }

        [JsonProperty("owner")]
        public string Owner { get; set; }

        [JsonProperty("filialId")]
        public int FilialId { get; set; }

        [JsonProperty("created")]
        public DateTimeOffset Created { get; set; }

        [JsonProperty("basketType")]
        public BasketType BasketType { get; set; }

        [JsonProperty("sumOut")]
        public decimal SumOut { get; set; }

        [JsonProperty("deliveryType")]
        public DeliveryType DeliveryType { get; set; }

        [JsonProperty(PropertyName = "courierName", Required = Required.AllowNull)]
        public string? CourierName { get; set; }
        //public string CourierPhone { get; set; } 

        [JsonProperty("pickupDate")]
        public DateTime PickupDate { get; set; }

        //public DateTime? OperReadyDate { get; set; }

        [JsonProperty("customerName", Required = Required.AllowNull)]
        public string? CustomerName { get; set; }

        [JsonProperty("customerPhone", Required = Required.AllowNull)]
        public string? CustomerPhone { get; set; }

        //public string FeedbackLevel1 { get; set; } 
        //public string FeedbackLevel2 { get; set; } 
        //public bool RemovePlastic { get; set; }
        //public bool UseSelfBag { get; set; }
        //public int NumberOfPersons { get; set; }

        [JsonProperty("orderPaymentType")]
        public OrderPaymentType OrderPaymentType { get; set; }
        //public decimal PaymentSumLimit { get; set; }
        //public PackageTypes? PackageType { get; set; }
        //public decimal TotalWeight { get; set; }
        //public bool IsChangedByDispatcher { get; set; }
        //public bool IsFilialChanged { get; set; }
        //public bool IsGift { get; set; }

        [JsonProperty(PropertyName = "notes", NullValueHandling = NullValueHandling.Ignore)]
        public string? Notes { get; set; }
        //public decimal MaxAvailableWeight { get; set; }
        //public int LimitGoods { get; set; }
        //public DateTime StartProductionTime { get; set; }
        //public DateTime EndProductionTime { get; set; }

        //public List<CollectableItemInfo> CollectableItems { get; set; }

        public List<FozzyCollectableItemInfo> CollectableItems { get; set; } = new List<FozzyCollectableItemInfo>();

        //public CollectableGlovoOrderInfo GlovoOrderInfo { get; set; }

        [JsonProperty("customerPhoneAlt", Required = Required.AllowNull)]
        public string? CustomerPhoneAlt { get; set; }

        [JsonProperty("timeSlotFrom")]
        public DateTime TimeSlotFrom { get; set; }
        
        [JsonProperty("timeSlotTo")]
        public DateTime TimeSlotTo { get; set; }


        //public string FeedbackLevel1UA { get; set; }
        //public string FeedbackLevel2UA { get; set; }

        [JsonProperty("contragentFullName", Required = Required.AllowNull)]
        public string? ContragentFullName { get; set; }
        
        [JsonProperty("contragentOKPO", Required = Required.AllowNull)]
        public string? ContragentOKPO { get; set; }

        [JsonProperty("clientAddress")]
        public string ClientAddress { get; set; }

        [JsonProperty("merchantId")]
        public int MerchantId { get; set; }

        [JsonProperty("merchantStateId", Required = Required.AllowNull)]
        public int? MerchantStateId { get; set; }
    }

    /*
    public class CollectableOrderInfo : ICollectableOrderInfo
    {
        //public OrderCollectingType OrderCollectingType { get; set; }
        public Guid BasketGuid { get; set; }
        public string OrderNumber { get; set; }
        public string OrderBarCode { get; set; }
        public string Owner { get; set; }
        public int FilialId { get; set; }
        public DateTimeOffset Created { get; set; }
        public BasketType BasketType { get; set; }
        public decimal SumOut { get; set; }
        public DeliveryType DeliveryType { get; set; }
        public string CourierName { get; set; } = "";
        public string CourierPhone { get; set; } = "";
        public DateTime PickupDate { get; set; }
        public DateTime? OperReadyDate { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public string FeedbackLevel1 { get; set; } = "";
        public string FeedbackLevel2 { get; set; } = "";
        public bool RemovePlastic { get; set; }
        public bool UseSelfBag { get; set; }
        public int NumberOfPersons { get; set; }
        public OrderPaymentType OrderPaymentType { get; set; }
        public decimal PaymentSumLimit { get; set; }
        public PackageTypes? PackageType { get; set; }
        public decimal TotalWeight { get; set; }
        public bool IsChangedByDispatcher { get; set; }
        public bool IsFilialChanged { get; set; }
        public bool IsGift { get; set; }
        public string Notes { get; set; }
        public decimal MaxAvailableWeight { get; set; }
        public int LimitGoods { get; set; }
        public DateTime StartProductionTime { get; set; }
        public DateTime EndProductionTime { get; set; }

        public List<CollectableItemInfo> CollectableItems { get; set; }

        public CollectableGlovoOrderInfo GlovoOrderInfo { get; set; }
        public string CustomerPhoneAlt { get; set; }
        public DateTime TimeSlotFrom { get; set; }
        public DateTime TimeSlotTo { get; set; }
        public string FeedbackLevel1UA { get; set; }
        public string FeedbackLevel2UA { get; set; }

        public string ContragentFullName { get; set; }
        public string ContragentOKPO { get; set; }
        public string ClientAddress { get; set; }
        public int MerchantId { get; set; }
        public int? MerchantStateId { get; set; }
        List<ICollectableItemInfo> ICollectableOrderInfo.CollectableItems
        {
            get
            {
                return CollectableItems.Cast<ICollectableItemInfo>().ToList();
            }
            set
            {
                CollectableItems = value.Cast<CollectableItemInfo>().ToList();
            }
        }

        ICollectableGlovoOrderInfo ICollectableOrderInfo.GlovoOrderInfo
        {
            get
            {
                return GlovoOrderInfo;
            }
            set
            {
                GlovoOrderInfo = (CollectableGlovoOrderInfo)value;
            }
        }
    }

    */
}
