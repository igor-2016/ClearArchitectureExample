using ApplicationServices.Interfaces.Models;
using ECom.Entities.Models;
using ECom.Types.ServiceBus;
using Entities.Consts;
using Entities.Models.Collecting;
using Entities.Models.Expansion;
using Expansion.Ecom.Clients.Options;
using Expansion.Interfaces.Clients;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Utils.Sys.Exceptions;
using Utils.Sys.RichHttpClient;
using Utils.Sys.RichHttpClient.Exceptions;
using Utils.Sys.RichHttpClient.Extensions;
using static Utils.Sys.RichHttpClient.Extensions.HttpClientExtensions;

namespace Expansion.Ecom.Clients
{

    public class WorkflowToExpansionClient : IWorkflowToExpansionClient
    {
        private readonly WorkflowToExpansionOptions _options;
        private readonly ILogger<IWorkflowToExpansionClient> _logger;
        private readonly HttpClient _httpClient;

        public WorkflowToExpansionClient(HttpClient httpClient, IOptions<WorkflowToExpansionOptions> options, 
            ILogger<IWorkflowToExpansionClient> logger)
        {
            _options = options.Value;
            _logger = logger;
            _httpClient = httpClient;   
        }

        public async Task<TraceableOrder> AcceptCollecting(Guid basketGuid, FozzyCollectableOrderInfo orderInfo, CancellationToken cancellationToken)
        {
            try
            {
                var methodName = ExpansionConsts.Common.App.Controllers.Workflow.WorkflowBaseRoute +
                    string.Format(_options.AccepCollectingMethodFormat, basketGuid);

                return await _httpClient
                     .SafePostAsync<FozzyCollectableOrderInfo, TraceableOrder, EComError>(methodName, orderInfo,
                     contentType: ContentTypeProvider.Json, cancellationToken: cancellationToken);
            }
            catch (PresentationException<EComError> ex)
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


        /// <summary>
        /// Вызов контроллера WorkflowController EnterCollecting
        /// </summary>
        /// <param name="ecomToExpansionOrder"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<TraceableOrder> EnterCollecting(FozzyCollectableOrderInfo ecomToExpansionOrder, 
            CancellationToken cancellationToken)
        {
            try
            {
                var methodName = ExpansionConsts.Common.App.Controllers.Workflow.WorkflowBaseRoute +
                    _options.EnterCollectingMethod;

                return await _httpClient
                     .SafePostAsync<FozzyCollectableOrderInfo, TraceableOrder, EComError>(methodName, ecomToExpansionOrder, 
                        contentType: ContentTypeProvider.Json, cancellationToken: cancellationToken);
            }
            catch (PresentationException<EComError> ex)
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

        public async Task<TraceableOrder> CancelCollecting(Guid basketGuid, CancellationToken cancellationToken)
        {
            try
            {
                var methodName = ExpansionConsts.Common.App.Controllers.Workflow.WorkflowBaseRoute +
                    string.Format(_options.CancelCollectingMethodFormat, basketGuid);

                return await _httpClient
                     .SafePostAsync<Unit, TraceableOrder, EComError>(methodName, Unit.Value,
                        contentType: ContentTypeProvider.Json, cancellationToken: cancellationToken);
            }
            catch (PresentationException<EComError> ex)
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

        public async Task<OrderData> SendOrderToFozzyCollectingService(Guid basketGuid, int newStatusId, CancellationToken cancellationToken)
        {
            try
            {
                var methodName = ExpansionConsts.Common.App.Controllers.Workflow.WorkflowBaseRoute +
                    string.Format(_options.SendOrderToFozzyCollectingServiceMethodFormat, basketGuid, newStatusId);

                return await _httpClient
                     .SafePostNoBodyAsync<OrderData, EComError>(methodName,
                     contentType: ContentTypeProvider.Json, cancellationToken: cancellationToken);
            }
            catch (PresentationException<EComError> ex)
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

        public async Task<OrderData> SendOrderToFozzyWebSite(Guid basketGuid, int newStatusId, CancellationToken cancellationToken)
        {
            try
            {
                var methodName = ExpansionConsts.Common.App.Controllers.Workflow.WorkflowBaseRoute +
                   string.Format(_options.SendOrderToFozzyWebSiteMethodFormat, basketGuid, newStatusId);

                return await _httpClient
                     .SafePostNoBodyAsync<OrderData, EComError>(methodName,
                     contentType: ContentTypeProvider.Json, cancellationToken: cancellationToken);
            }
            catch (PresentationException<EComError> ex)
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
