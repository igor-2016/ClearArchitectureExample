using Newtonsoft.Json;

namespace Collecting.Interfaces.Clients.Responses
{
    public class FozzyCustomParams
    {
        public Guid? lineId {get; set;}

        public Guid? basketGuid { get; set; }

        public bool? isWeighted { get; set; }   

        [JsonIgnore]
        public string error { get; set; }
    }
}
