using ECom.Types.Delivery;
using ECom.Types.EcomCatalog.Requests;

namespace Catalog.Interfaces.Dto.Requests
{
    public class CatalogItemsRequest : ISimpleCatalogItemsRequest
    {
        public static CatalogItemsRequest GetSimpleRequest(int lagerId, int filialId, int merchantId, DeliveryType deliveryType = DeliveryType.Unknown, Guid? basketGuid = null)
        {
            return new CatalogItemsRequest()
            {
                DeliveryType = deliveryType,
                SkuId = lagerId,
                FilialId = filialId,
            };
        }

        public string CustomFilter { get; set; }
        public int CategoryId { get; set; }
        public string PromoId { get; set; }
        public List<int> UniversalFilters { get; set; }
        public Dictionary<int, Tuple<int, int>> RangeFilters { get; set; }
        public Dictionary<int, List<int>> MultiFilters { get; set; }
        public string SortBy { get; set; }
        public List<int> CategoryFilter { get; set; }
        public bool OnlyPromo { get; set; }
        public int SkuId { get; set; }
        public List<int> Skus { get; set; }
        public List<string> Promos { get; set; }
        public List<string> Sets { get; set; }
        public int From { get; set; }
        public int To { get; set; }
        public int AtlantPromo { get; set; }
        public List<int> AtlantPromos { get; set; }
        public bool EcoPacking { get; set; }
        public bool Ingredients { get; set; }
        public List<string> Slugs { get; set; }
        public double? FromPrice { get; set; }
        public double? ToPrice { get; set; }
        public string Barcode { get; set; }
        public bool IncludeClosedAssortment { get; set; }
        public bool? DisplayOnFozzyShopSite { get; set; }
        public int FilialId { get; set; }
        public DeliveryType DeliveryType { get; set; }
        public Guid? BasketGuid { get; set; }
        public int MemberId { get; set; } //  недавно добавилось
    }
}
