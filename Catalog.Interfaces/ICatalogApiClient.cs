using Catalog.Interfaces.Dto.Requests;
using Catalog.Interfaces.Dto.Responses;

namespace Catalog.Interfaces
{
    public interface ICatalogApiClient
    {
        Task<CatalogItemsResponse?> GetCatalogItems(CatalogItemsRequest catalogItemsRequest, CancellationToken cancellationToken);
    }
}