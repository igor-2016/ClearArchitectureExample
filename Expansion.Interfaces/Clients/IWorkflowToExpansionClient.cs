using ApplicationServices.Interfaces.Models;
using ECom.Entities.Models;
using Entities.Models.Collecting;
using Entities.Models.Expansion;

namespace Expansion.Interfaces.Clients
{
    public interface IWorkflowToExpansionClient
    {
        Task<TraceableOrder> EnterCollecting(FozzyCollectableOrderInfo ecomToExpansionOrder, 
            CancellationToken cancellationToken);

        Task<TraceableOrder> AcceptCollecting(Guid basketGuid, FozzyCollectableOrderInfo orderInfo,
            CancellationToken cancellationToken);

        Task<TraceableOrder> CancelCollecting(Guid basketGuid, CancellationToken cancellationToken);

        Task<OrderData> SendOrderToFozzyWebSite(Guid basketGuid, int newStatusId,
            CancellationToken cancellationToken);

        Task<OrderData> SendOrderToFozzyCollectingService(Guid basketGuid, int newStatusId,
            CancellationToken cancellationToken);
    }
}
