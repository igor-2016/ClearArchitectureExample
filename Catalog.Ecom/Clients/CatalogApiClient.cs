using Catalog.Ecom.Options;
using Catalog.Interfaces;
using Catalog.Interfaces.Dto.Requests;
using Catalog.Interfaces.Dto.Responses;
using ECom.Types.ServiceBus;
using Microsoft.Extensions.Options;
using Utils.Sys.RichHttpClient.Extensions;

namespace Catalog.Ecom.Clients
{

    public class CatalogApiClient : ICatalogApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly CatalogApiOptions _options;

        public CatalogApiClient(HttpClient httpClient, IOptions<CatalogApiOptions> options)
        {
            _httpClient = httpClient;
            _options = options.Value;
        }

        public async Task<CatalogItemsResponse?> GetCatalogItems(CatalogItemsRequest catalogItemsRequest, CancellationToken cancellationToken)
        {
            return await _httpClient.SafePostAsync<CatalogItemsRequest, CatalogItemsResponse, EComError>(_options.GetItemsMethod, 
                catalogItemsRequest, cancellationToken);
        }
    }
}
