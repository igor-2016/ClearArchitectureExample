using ECom.Types.Responses;
using ECom.Types.ServiceBus;
using Newtonsoft.Json;

namespace ApplicationServices.Interfaces.Responses
{
    public class DefaultDataResponse<T> : IDefaultEComResponse
    {
        public DefaultDataResponse(T view)
        {
            EComError = EComErrors.OK;
            Data = view;
        }

        [JsonProperty("eComError")]
        public EComError EComError { get; set; }

        [JsonProperty("data")]
        public T Data { get; set; }
    }
}
