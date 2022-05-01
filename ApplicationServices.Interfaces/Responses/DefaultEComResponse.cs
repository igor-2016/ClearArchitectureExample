using ECom.Types.Responses;
using ECom.Types.ServiceBus;

namespace ApplicationServices.Interfaces.Responses
{
    public class DefaultEComResponse : IDefaultEComResponse
    {
        public EComError EComError { get; set; }
    }
}
