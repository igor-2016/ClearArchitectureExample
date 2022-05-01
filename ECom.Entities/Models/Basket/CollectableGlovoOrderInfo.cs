using ECom.Types.Collect;

namespace ECom.Entities.Models
{
    public class CollectableGlovoOrderInfo : ICollectableGlovoOrderInfo
    {
        public string PickUpCode { get; set; }
        public string GlovoOrderId { get; set; }
        public string GlovoOrderCode { get; set; }
        public string CourierName { get; set; }
        public string CourierPhone { get; set; }
    }
}
