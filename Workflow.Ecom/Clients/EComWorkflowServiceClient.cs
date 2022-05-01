using ECom.Entities.Models.Workflow;
using ECom.Types.Exceptions;
using ECom.Types.ServiceBus;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using Utils.Sys.Exceptions;
using Utils.Sys.RichHttpClient;
using Utils.Sys.RichHttpClient.Exceptions;
using Utils.Sys.RichHttpClient.Extensions;
using Workflow.Ecom.Clients.Options;
using Workflow.Interfaces.Clients;
using Workflow.Interfaces.Clients.Responses;
using Workflow.Interfaces.Exceptions;
using static Utils.Sys.RichHttpClient.Extensions.HttpClientExtensions;

namespace Workflow.Ecom.Clients
{

    public class EComWorkflowServiceClient : IWorkflowServiceClient
    {
        private readonly EComWorkflowServiceOptions _options;
        private readonly ILogger<IWorkflowServiceClient> _logger;
        private readonly HttpClient _httpClient;

        public EComWorkflowServiceClient(HttpClient httpClient, 
            IOptions<EComWorkflowServiceOptions> options, 
            ILogger<IWorkflowServiceClient> logger)
        {
            _options = options.Value;
            _logger = logger;
            _httpClient = httpClient;    
        }

        public async Task<EmptyDataResult> OnChangeState<TData>(Guid basketId, int currentStateId, int nextStateId, TData data, CancellationToken cancellationToken)
        {
            var loggingMessagePrefix = "Call workflow OnChangeState";
            return await OnChangeState(
                data,
                string.Format(_options.TranformMethodFormat, basketId, currentStateId, nextStateId),
                loggingMessagePrefix,
                cancellationToken
                );
        }

        /// <summary>
        /// Для асинхронной проверки ECom.Workflow о текущем исполнении по ордеру
        /// </summary>
        /// <param name="basketId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<WorkflowOrderState> GetOrderCurrentState(Guid basketId, CancellationToken cancellationToken)
        {
            return await _httpClient
                    .SafeGetAsync<WorkflowOrderState, EComError>(string.Format(_options.GetCurrentStateMethodFormat, basketId), 
                        contentType: ContentTypeProvider.Json, cancellationToken: cancellationToken);
        }

        public async Task<EmptyDataResult> OnChangeStateNoData(Guid basketId, int currentStateId, int nextStateId, CancellationToken cancellationToken)
        {
            return await OnChangeState(basketId, currentStateId, nextStateId, string.Empty, cancellationToken);
        }

        private async Task<EmptyDataResult> OnChangeState<TData>(
            TData data,
            string completedQueryString,
            string loggingMessagePrefix, 
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("{msg}", loggingMessagePrefix + " started");
            try
            {

                await _httpClient
                     .SafePostAsync<TData, EComError>(completedQueryString, data, contentType: ContentTypeProvider.Json, cancellationToken: cancellationToken);

                return EmptyDataResult.Success(HttpStatusCode.OK);
            }
            catch (PresentationException<EComError> ex)
            {
                _logger.LogError("{msg} {error} {statusCode}", loggingMessagePrefix + " fail (presentation)", ex.Message, ex.StatusCode);
                return EmptyDataResult.Error(WorkflowErrors.NotFoundMapFromEComToFozzyPaymentType, ex.Message ?? "Unknown error", ex.StatusCode, ex);
            }
            catch (RichHttpClientException ex)
            {
                _logger.LogError("{msg} {error} {statusCode}", loggingMessagePrefix + " fail (rich)", ex.FullMessage(), ex.Code);
                return EmptyDataResult.Error(WorkflowErrors.UnhandledExceptionOnChangeWorkflowStatus, ex.FullMessage(), ex.Code ?? 0, ex);
            }
            catch (Exception ex)
            {
                _logger.LogError("{msg} {error} {StackTrace}", loggingMessagePrefix + " error (general)", ex.FullMessage(), ex.StackTrace);
                return EmptyDataResult.Error(WorkflowErrors.UnhandledExceptionOnChangeWorkflowStatus, ex);
            }
        }

       
    }
}
