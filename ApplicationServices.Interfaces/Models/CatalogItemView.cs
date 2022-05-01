using Collecting.Interfaces.Enums;
using Newtonsoft.Json;

namespace ApplicationServices.Interfaces.Models
{
    public class CatalogItemView
    {

        public CatalogItemView()
        {
            Prices = new CatalogItemPricesView();
        }

        //public CatalogItemView(CatalogItem  fromItem)
        //{

        //}

        [JsonProperty("lagerId")]
        public int Id { get; set; }

        [JsonProperty("isActivityEnable")]
        public bool? IsActivityEnable { get; set; }

        [JsonProperty("freezeStatus")]
        public FozzyFreezeStatus? FreezeStatus { get; set; }


        [JsonProperty("name")]
        public string LagerName { get; set; }

        [JsonProperty("unit")]
        public string LagerUnit { get; set; }

        [JsonProperty("isWeighted")]
        public bool? IsWeighted { get; set; }

        
        [JsonProperty("сollectable")]
        public bool Collectable { get; set; } 

        /// <summary>
        /// Где искать товар
        /// </summary>
        [JsonProperty("locationCategory")]
        public string SortingCategory { get; set; }

        [JsonProperty("prices")]
        public CatalogItemPricesView Prices { get; set; }

    }
}
