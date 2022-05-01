using Newtonsoft.Json;

namespace Entities.Models.Collecting
{
    public class CustomParams
    {
        public Guid? lineId {get; set;}

        public Guid? basketGuid { get; set; }

        public bool? isWeighted { get; set; }   

        [JsonIgnore]
        public string error { get; set; }
    }
}
