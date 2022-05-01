using Collecting.Fozzy.Clients.Options;
using Collecting.Interfaces;
using Collecting.Interfaces.Clients;
using Collecting.Interfaces.Clients.Responses;
using ECom.Types.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mime;
using Utils.Sys.Exceptions;
using Utils.Sys.RichHttpClient;
using Utils.Sys.RichHttpClient.Exceptions;
using Utils.Sys.RichHttpClient.Extensions;

namespace Collecting.Fozzy.Clients
{

    public class FozzyStaffServiceClient : IFozzyStaffServiceClient
    {
        private readonly FozzyShopStaffServiceOptions _options;
        private readonly ILogger<IFozzyStaffServiceClient> _logger;
        private readonly HttpClient _httpClient;

        public FozzyStaffServiceClient(HttpClient httpClient, IOptions<FozzyShopStaffServiceOptions> options, 
            ILogger<IFozzyStaffServiceClient> logger)
        {
            _options = options.Value;
            _logger = logger;
            _httpClient = httpClient;   
        }


        public async Task<GetFozzyStaffResult> GetStaffInfoByInn(string inn, CancellationToken cancellationToken)
        {
            //"/GetEmployeeInfo?peopleINN=" + inn
            return await GetStaffInfoBy(string.Format(_options.GetByInnMethodFormat, inn),
                ContentTypeProvider.Json,
                "GetEmployeeInfo by INN",
                CollectErrors.NotFoundEmployeeByGlobalUserId,
                CollectErrors.InvalidFormatEmployeeByGlobalUserId,
                CollectErrors.PublicErrorGetEmployeeByGlobalUserId,
                CollectErrors.UnhandledErrorGetEmployeeByGlobalUserId,
                cancellationToken);
        }

        public async Task<GetFozzyStaffResult> GetStaffInfoByGlobalUserId(int globalUserId, 
            CancellationToken cancellationToken)
        {
            return await GetStaffInfoBy(string.Format(_options.GetByGlobalIdMethodFormat, globalUserId),
                ContentTypeProvider.Json,
                "GetEmployeeInfo by globalUserId",
                CollectErrors.NotFoundEmployeeByGlobalUserId,
                CollectErrors.InvalidFormatEmployeeByGlobalUserId,
                CollectErrors.PublicErrorGetEmployeeByGlobalUserId,
                CollectErrors.UnhandledErrorGetEmployeeByGlobalUserId, cancellationToken);
        }

        protected virtual async Task<GetFozzyStaffResult> GetStaffInfoBy(string completedQueryString,
            ContentType defaultContentType,
            string loggingMessagePrefix, 
            CollectErrors notFoundError,
            CollectErrors formatErrorCode,
            CollectErrors publicErrorCode, 
            CollectErrors unhandledError, CancellationToken cancellationToken)
        {
            _logger.LogInformation("{msg}", loggingMessagePrefix + " started");
            try
            {
                var staff = await _httpClient
                     .SafeGetAsync<FozzyStaff, FozzyClientConfirmResponse>(completedQueryString, contentType: defaultContentType, cancellationToken: cancellationToken);

                if (staff == null || staff.ConfirmResponse == null || staff.EmployeeInfo == null)
                {
                    _logger.LogError("{msg} {error} {code}", loggingMessagePrefix + " fozzy shop service error", "Not found!", 404);
                    return GetFozzyStaffResult.ErrorPublic("Нет ответа по указанному формату", formatErrorCode, HttpStatusCode.NotFound);
                }
                else
                {
                    if (!string.IsNullOrEmpty(staff.EmployeeInfo.peopleINN))
                    {
                        _logger.LogInformation("{msg}", loggingMessagePrefix + " success");
                        return GetFozzyStaffResult.Success(staff);
                    }
                    else
                    {
                        _logger.LogWarning("{msg}", loggingMessagePrefix + " not found");
                        return GetFozzyStaffResult.NotFound(staff.ConfirmResponse.errorMessage, notFoundError);
                    }
                }
            }
            catch (PresentationException<FozzyClientConfirmResponse> ex)
            {
                _logger.LogError("{msg} {error} {statusCode}", loggingMessagePrefix + " fail (presentation)", ex.ErrorModel?.errorMessage, ex.StatusCode);
                return GetFozzyStaffResult.ErrorPublic(ex.ErrorModel?.errorMessage ?? "Unknown error", publicErrorCode, ex.StatusCode, ex);
            }
            catch (RichHttpClientException ex)
            {
                _logger.LogError("{msg} {error} {statusCode}", loggingMessagePrefix + " fail (rich)", ex.FullMessage(), ex.Code);
                return GetFozzyStaffResult.Error(unhandledError, ex.Code ?? 0, ex.FullMessage(), ex);
            }
            catch (Exception ex)
            {
                _logger.LogError("{msg} {error} {StackTrace}", loggingMessagePrefix + " error (general)", ex.FullMessage(), ex.StackTrace);
                return GetFozzyStaffResult.Error(unhandledError, ex);
            }
        }

        
    }
}
