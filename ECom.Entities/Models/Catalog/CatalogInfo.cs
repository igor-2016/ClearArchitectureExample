using ECom.Types.Delivery;
using ECom.Types.Orders;
using Newtonsoft.Json;

namespace ECom.Entities.Models
{
    /// <summary>
    /// Поиск товаров по LagerId
    /// </summary>
    public class CatalogInfo
    {
        /// <summary>
        /// Код товара
        /// </summary>
        [JsonProperty("lagerId")]
        public int LagerId { get; set; }

        /// <summary>
        /// Филиал
        /// </summary>
        public int FilialId { get; set; }

        /// <summary>
        /// Тип доставки в ECom
        /// </summary>
        public DeliveryType DeliveryType { get; set; } = DeliveryType.Unknown;

        /// <summary>
        /// Мерчант (пок не реализован!!)
        /// </summary>
        public Merchant Merchant { get; set; }

        /// <summary>
        /// Код корзинки ECom (пока не используется)
        /// </summary>
        public Guid? BasketGuid { get; set; }


        /// <summary>
        /// Список найденых товаров
        /// </summary>
        public List<CatalogItem> Items { get; set; } = new List<CatalogItem>();

        /// <summary>
        /// Предполагается, что нашли в каталоге один товар по критериям поиска в идеале
        /// </summary>
        /// <returns></returns>
        public CatalogItem? GetSingleItem()
        {
            return Items.Count == 1 ? Items[0] : null;
        }
    }
}
