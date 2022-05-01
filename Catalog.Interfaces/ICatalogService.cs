using ECom.Entities.Models;
using Entities.Models.Expansion;

namespace Catalog.Interfaces
{
    public interface ICatalogService
    {
        //Task<CatalogResponse> GetGoodsData(LookupItemRequest req);

        
        Task<CatalogInfo> GetCatalogItems(int lagerId, int filialId, int merchantId,
            CancellationToken cancellationToken,
            int ecomDeliveryType = 0, Guid? basketGuid = null);

        Task<CatalogItem> GetCatalogItem(TraceableOrder order, int? lagerId, CancellationToken cancellationToken);
        
    }
}