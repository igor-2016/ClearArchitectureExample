using Newtonsoft.Json;

namespace ApplicationServices.Interfaces.Models
{
    /// <summary>
    /// Цены товара в каталоге
    /// </summary>
    public class CatalogItemPricesView
    {
        /// <summary>
        /// Текущая цена розницы
        /// </summary>
        [JsonProperty("price")]
        public decimal Price { get; set; }

        /// <summary>
        /// Текущая оптовая цена 
        /// </summary>
        [JsonProperty("priceOpt")]
        public decimal? PriceOpt { get; set; }

        /// <summary>
        /// Старая цена (опт или роз?)
        /// </summary>
        [JsonProperty("priceOld")]
        public decimal? PriceOld { get; set; }

    }
}
