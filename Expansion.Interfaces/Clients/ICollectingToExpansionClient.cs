using Entities.Models.Collecting;
using Expansion.Interfaces.Clients.Responses;

namespace Expansion.Interfaces.Clients
{
    public interface ICollectingToExpansionClient
    {
        Task<ConfirmResponse> ChangeCollecting(OrderData fozzyOrder, CancellationToken cancellationToken);
    }
}
