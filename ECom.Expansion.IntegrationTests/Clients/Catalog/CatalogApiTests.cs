using Catalog.Interfaces.Dto.Requests;
using Catalog.Interfaces.Dto.Responses;
using ECom.Expansion.TestHelpers;
using ECom.Types.ServiceBus;
using HealthCheck.Controllers.Catalog.Requests;
using System;
using System.Threading;
using System.Threading.Tasks;
using Utils.Sys.Exceptions;
using Utils.Sys.RichHttpClient.Extensions;
using Xunit;

namespace ECom.Expansion.IntegrationTests.Clients.Catalog
{

    public class CatalogApiTests
    {
        [Fact]
        public async Task Can_GetSimpleCatalogItems()
        {
            var cancellationToken = CancellationToken.None;
            var data = CheckCatalogRequest.Default;
            var request = CatalogItemsRequest.GetSimpleRequest(
                data.LagerId, data.Filial, (int)data.Merchant, data.EComDeliveryType, null);

            var validPath = "/api/Dev/GetSimpleCatalogItems";
            var baseUrl = "https://catalog.ecom.test.fz.lan/";
            var timeout = TimeSpan.Parse("00:00:10");

            using (var httpClient = HttpClientHelper.GetHttpClient(baseUrl, timeout))
            {
                var response = await httpClient
                    .SafePostAsync<CatalogItemsRequest, CatalogItemsResponse, EComError>(validPath, request, cancellationToken);
                var result = response.GetSimpleCatalogItemsResponse(request);
                Assert.NotNull(result);
                Assert.Equal(data.LagerId, result.LagerId);
                Assert.Equal(data.Filial, result.FilialId);
                //Assert.Equal(data.MerchantId, result.Merchant);
                Assert.Equal(data.EComDeliveryType, result.DeliveryType);

                Assert.Single(result.Items);
                var item = result.Items[0];
                Assert.Equal(data.LagerId, item.Id);
                Assert.False(item.IsWeighted);
                Assert.NotEmpty(item.Name);
                Assert.NotEmpty(item.Unit);
                Assert.NotEmpty(item.NameForSite);
                Assert.NotEmpty(item.SortingCategory);
            }
        }

        [Fact]
        public void Cannot_GetSimpleCatalogItems_NotFound()
        {
            CancellationToken cancellationToken = new CancellationToken();  
            var data = CheckCatalogRequest.Default;
            var request = CatalogItemsRequest.GetSimpleRequest(
                data.LagerId, data.Filial, (int)data.Merchant, data.EComDeliveryType, null);

            var getItemsPath_Wrong = "/api/Dev/GetSimpleCatalogItems3r324dfg";
            var baseUrl = "https://catalog.ecom.test.fz.lan/";
            var timeout = TimeSpan.Parse("00:00:10");

            using (var httpClient = HttpClientHelper.GetHttpClient(baseUrl, timeout))
            {
                _ = Assert.ThrowsAnyAsync<NotFoundException>(
                    () => httpClient.SafePostAsync<CatalogItemsRequest, CatalogItemsResponse>(getItemsPath_Wrong, request, cancellationToken));
            }
        }
    }
}
