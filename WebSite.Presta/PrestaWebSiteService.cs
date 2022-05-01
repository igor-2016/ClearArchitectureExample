using DomainServices.Interfaces;
using Entities.Models.Collecting;
using Entities.Models.Expansion;
using WebSiteService.Interfaces;
using WebSiteService.Interfaces.Clients;

namespace WebSite.Presta
{
    public class PrestaWebSiteService : IWebSiteService
    {
        private readonly IFozzyShopSiteServiceClient _fozzyShopSiteServiceClient;

        private readonly ITransformService _transformService;

        public PrestaWebSiteService(
            IFozzyShopSiteServiceClient fozzyShopSiteServiceClient,
            ITransformService transformService
            )
        {
            _fozzyShopSiteServiceClient = fozzyShopSiteServiceClient;
            _transformService = transformService;
        }


        public async Task<OrderData> SendOrderToFozzyWebSite(TraceableOrder traceableOrder, 
            CancellationToken cancellationToken)
        {
            var orderToSend = _transformService.ToFozzyOrder(traceableOrder);
            var result = await _fozzyShopSiteServiceClient.PutOrderData(orderToSend, cancellationToken);
            if (!result.IsSuccess)
                throw result.AnError;
            return orderToSend;
        }
    }
}