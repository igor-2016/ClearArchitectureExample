using Collecting.Fozzy.Clients.Options;
using Collecting.Interfaces.Clients;
using Collecting.Interfaces.Clients.Responses;
using ECom.Types.Exceptions;
using Entities.Models.Collecting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Utils;
using Utils.Sys.RichHttpClient;
using Utils.Sys.RichHttpClient.Extensions;

namespace Collecting.Fozzy.Clients
{

    public class FozzyCollectingServiceClient : IFozzyCollectingServiceClient
    {
        private readonly FozzyShopCollectingServiceOptions _options;
        private readonly ILogger<IFozzyCollectingServiceClient> _logger;
        private readonly HttpClient _httpClient;

        public FozzyCollectingServiceClient(HttpClient httpClient, IOptions<FozzyShopCollectingServiceOptions> options, 
            ILogger<IFozzyCollectingServiceClient> logger)
        {
            _options = options.Value;
            _logger = logger;
            _httpClient = httpClient;
        }

        public async Task<GetFozzyOrderDataResult> GetOrderData(string orderId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("{msg}", "GetOrderData started");
            try
            { 
                var order = await _httpClient.SafeGetAsync<FozzyOrderData, FozzyClientConfirmResponse>(
                    string.Format(_options.GetByOrderMethodFormat, orderId), contentType: ContentTypeProvider.Xml, 
                    cancellationToken: cancellationToken);

                return GetFozzyOrderDataResult.Success(order);
            }
            catch (Exception ex)
            {
                _logger.LogError("{msg} {error} {StackTrace}", "GetOrderData error", ex.FullMessage(), ex.StackTrace);
                return GetFozzyOrderDataResult.Error(ex);
            }
        }

        public async Task<PutFozzyOrderDataResult> PutOrderData(FozzyOrderData orderData, CancellationToken cancellationToken)
        {
            _logger.LogInformation("{msg}", "PutOrderData started");
            try
            {
                var response = await _httpClient.SafePostAsync<FozzyOrderData, FozzyClientConfirmResponse, FozzyClientConfirmResponse>(
                    _options.PutOrderMethod, orderData, contentType: ContentTypeProvider.Json, cancellationToken: cancellationToken);

                if (response.errorCode == FozzyClientConfirmResponse.OK)
                {
                    _logger.LogInformation("{msg}", "PutOrderData success");
                    return PutDataResult.Success<PutFozzyOrderDataResult>();
                }
                else
                {
                    _logger.LogError("{msg} {error} {code}", "PutOrderData fozzy shop service error", response.errorMessage, response.errorCode);
                    return PutDataResult.Error<PutFozzyOrderDataResult>(new Exception($"{response.errorMessage}, code:{response.errorCode}"));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("{msg} {error} {StackTrace} {data}", "PutOrderData error", ex.FullMessage(), ex.StackTrace, orderData.ToXmlUtf8());
                return PutDataResult.Error<PutFozzyOrderDataResult>(ex);
            }
            /*
            _logger.LogInformation("{msg}", "PutOrderData started");
            try
            {
                var message = orderData.ToJson();

                _logger.LogInformation("{msg} {data}", "PutOrderData json prepared", message);

                var content = new StringContent(message, Encoding.UTF8, "application/json");

                using (var client = new HttpClient())
                {
                    using (var response = await client.PostAsync(_options.BaseUrl + "/PutOrderData", content))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            var xmlOrJsonResponse = await response.Content.ReadAsStringAsync();

                            var result = xmlOrJsonResponse.ToObj<FozzyClientConfirmResponse>(response.Content.Headers.ContentType.MediaType);

                            if (result.errorCode == FozzyClientConfirmResponse.OK)
                            {
                                _logger.LogInformation("{msg}", "PutOrderData success");
                                return PutDataResult.Success<FozzyShopPutOrderDataResult>(response.StatusCode);
                            }
                            else
                            {
                                _logger.LogError("{msg} {error} {code}", "PutOrderData fozzy shop service error", result.errorMessage, result.errorCode);
                                return PutDataResult.Error<FozzyShopPutOrderDataResult>(new Exception($"{result.errorMessage}, code:{result.errorCode}"));
                            }
                        }

                        _logger.LogError("{msg} {error} {statusCode}", "PutOrderData fail", response.ReasonPhrase, response.StatusCode);
                        return PutDataResult.Fail<FozzyShopPutOrderDataResult>(response.StatusCode, response.ReasonPhrase);
                    }
                }
            }
            catch(Exception ex)
            {
                _logger.LogError("{msg} {error} {StackTrace}", "PutOrderData error", ex.FullMessage(), ex.StackTrace);
                return PutDataResult.Error<FozzyShopPutOrderDataResult>(ex);
            }
            */
        }
    }
}
