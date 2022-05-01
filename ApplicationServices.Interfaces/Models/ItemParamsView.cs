using Newtonsoft.Json;

namespace ApplicationServices.Interfaces.Models
{
    public class ItemParamsView
    {
        [JsonProperty("lineId")]
        public Guid? LineId { get; set; }

        [JsonProperty("basketGuid")]
        public Guid? BasketGuid { get; set; }

        [JsonProperty("isWeighted")]
        public bool? IsWeighted { get; set; }

        [JsonProperty("error")]
        public string Error { get; set; }
    }
}
