using Newtonsoft.Json;

namespace Entities.Models.Expansion
{
    public class ItemParams
    {
        [JsonProperty("lineId")]
        public Guid? LineId { get; set; }

        [JsonProperty("basketGuid")]
        public Guid? BasketGuid { get; set; }

        [JsonProperty("isWeighted")]
        public bool? IsWeighted { get; set; }

        [JsonIgnore]
        public string Error { get; set; }
    }
}
