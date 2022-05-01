using Entities.Models.Collecting;
using WebSiteService.Interfaces.Clients.Responses;

namespace WebSiteService.Interfaces.Clients
{
    public interface IFozzyShopSiteServiceClient
    {
        Task<PutDataResult> PutOrderData(OrderData orderData, CancellationToken cancellationToken);
    }
}
