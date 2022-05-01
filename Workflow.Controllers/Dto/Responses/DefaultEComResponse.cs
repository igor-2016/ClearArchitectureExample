using ECom.Types.Responses;
using ECom.Types.ServiceBus;

namespace Workflow.Controllers.Dto.Responses
{
    public class DefaultEComResponse : IDefaultEComResponse
    {
        public EComError EComError { get; set; }
    }
}
