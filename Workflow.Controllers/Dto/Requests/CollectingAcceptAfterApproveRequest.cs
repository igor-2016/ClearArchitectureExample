using ECom.Entities.Models;
using ECom.Types.Collect;
using Workflow.Interfaces.Requests;

namespace Workflow.Controllers.Dto.Requests
{
    public class CollectingAcceptAfterApproveRequest : ICollectingAcceptAfterApproveRequest
    {
        public Guid BasketGuid { get; set; }
        public FozzyCollectableOrderInfo AcceptedOrder { get; set; }
    }
}
