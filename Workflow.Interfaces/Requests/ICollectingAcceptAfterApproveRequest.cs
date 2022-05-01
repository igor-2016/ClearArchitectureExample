using ECom.Entities.Models;
using ECom.Types.Collect;
using Entities.Models;

namespace Workflow.Interfaces.Requests
{
    public interface ICollectingAcceptAfterApproveRequest : IQuery
    {
        Guid BasketGuid { get; set; }

        FozzyCollectableOrderInfo AcceptedOrder { get; set; }
    }
}
