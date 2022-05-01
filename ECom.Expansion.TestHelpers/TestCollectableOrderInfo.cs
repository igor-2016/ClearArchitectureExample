using ECom.Types;
using ECom.Types.Collect;
using ECom.Types.Delivery;
using ECom.Types.Orders;
using Temabit.FZFrameworkStandard.FZTypes.Messaging.Entities;

namespace ECom.Expansion.TestHelpers
{
    //public class TestNewEComCollectableOrder : INewEComCollectableOrder
    //{
    //    public ICollectableOrderInfo CollectableOrderInfo { get; set; }
    //    public OrderCollectingType OrderCollectingType { get; set; }
    //}

    public class TestCollectableOrderInfo : ICollectableOrderInfo
    {
        public TestCollectableOrderInfo()
        {
            CollectableItems = new List<ICollectableItemInfo>();
        }
        public OrderCollectingType OrderCollectingType { get; set; }
        public Guid BasketGuid { get; set; }
        public string OrderNumber { get; set; }
        public string OrderBarCode { get; set; }
        public string Owner { get; set; }
        public int FilialId { get; set; }
        public DateTimeOffset Created { get; set; }
        public BasketType BasketType { get; set; }
        public decimal SumOut { get; set; }
        public DeliveryType DeliveryType { get; set; }
        public string CourierName { get; set; }
        public string CourierPhone { get; set; }
        public DateTime PickupDate { get; set; }
        public DateTime? OperReadyDate { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public string FeedbackLevel1 { get; set; }
        public string FeedbackLevel2 { get; set; }
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
        public List<ICollectableItemInfo> CollectableItems { get; set; }
        public ICollectableGlovoOrderInfo GlovoOrderInfo { get; set; }

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

        
    }

    public class TestCollectableItemInfo : ICollectableItemInfo
    {
        public TestCollectableItemInfo()
        {
            Attributes = new HashSet<BasketItemAttributes>();
        }

        public Guid Id { get; set; }
        public Guid? ParentId { get; set; }
        public int LagerId { get; set; }
        public decimal Qty { get; set; }
        public string Barcode { get; set; }
        public string Title { get; set; }
        public decimal PriceOut { get; set; }
        public decimal PriceRozn { get; set; }
        public decimal SumRozn { get; set; }
        public decimal SumOut { get; set; }
        public HashSet<BasketItemAttributes> Attributes { get; set; }
        public string Comment { get; set; }
        public bool IsChangedByDispatcher { get; set; }
        public DateTimeOffset? Modified { get; set; }
        public decimal? MaxQty { get; set; }
        public GoodsData GoodsData { get; set; }
        public int? ReplacementOnLagerId { get; set; }

        public int[] ReplacementLagers { get; set; } = new int[0];
    }
}
