using Entities.Consts;
using Entities.Models.Collecting;
using Expansion.Ecom.Clients.Options;
using Expansion.Interfaces.Clients;
using Expansion.Interfaces.Clients.Responses;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text;
using Utils.Sys.Exceptions;
using Utils.Sys.RichHttpClient;
using Utils.Sys.RichHttpClient.Exceptions;
using Utils.Sys.RichHttpClient.Extensions;

namespace Expansion.Ecom.Clients
{

    public class CollectingToExpansionClient : ICollectingToExpansionClient
    {
        private readonly CollectingToExpansionOptions _options;
        private readonly ILogger<ICollectingToExpansionClient> _logger;
        private readonly HttpClient _httpClient;

        public CollectingToExpansionClient(HttpClient httpClient, IOptions<CollectingToExpansionOptions> options, 
            ILogger<ICollectingToExpansionClient> logger)
        {
            _options = options.Value;
            _logger = logger;
            _httpClient = httpClient;   
        }


        public async Task<ConfirmResponse> ChangeCollecting(OrderData fozzyOrder, CancellationToken cancellationToken)
        {
            try
            {
                var methodName = ExpansionConsts.Common.App.Controllers.Collecting.CollectingBaseRoute +
                    _options.ChangeCollectingMethod;

                return await _httpClient
                        .SafePostAsync<OrderData, ConfirmResponse, ConfirmResponse>(methodName, fozzyOrder, 
                            contentType: ContentTypeProvider.Xml, encoding: Encoding.UTF8, cancellationToken: cancellationToken);
            }
            catch (PresentationException<ConfirmResponse> ex)
            {
                throw;
            }
            catch (RichHttpClientException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
