using Newtonsoft.Json;

namespace ECom.Entities.Models
{

    public class FozzyCollectableItemInfo 
    {
        public Guid Id { get; set; }
        //public Guid? ParentId { get; set; }

        [JsonProperty("lagerId")]
        public int LagerId { get; set; }
        
        [JsonProperty("qty")]
        public decimal Qty { get; set; }
        //public string Barcode { get; set; }
        //public string Title { get; set; }
        
        [JsonProperty("priceOut")]
        public decimal PriceOut { get; set; }

        //public decimal PriceRozn { get; set; }
        //public decimal SumRozn { get; set; }
        
        [JsonProperty("sumOut")]
        public decimal SumOut { get; set; }
        //public HashSet<BasketItemAttributes> Attributes { get; set; }
        //public string Comment { get; set; }
        //public bool IsChangedByDispatcher { get; set; }

        [JsonProperty("modified", Required = Required.AllowNull)]
        public DateTimeOffset? Modified { get; set; }
        //public decimal? MaxQty { get; set; }
        //public GoodsData GoodsData { get; set; }

        [JsonProperty("replacementOnLagerId", Required = Required.AllowNull)]
        public int? ReplacementOnLagerId { get; set; }

        [JsonProperty("replacementLagers")]
        public int[] ReplacementLagers { get; set; }
    }

    /*
    public class CollectableItemInfo : ICollectableItemInfo
    {
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
        public int[] ReplacementLagers { get; set; }
    }
    */
}
