using DomainServices.Interfaces;
using ECom.Types.Exceptions;
using Entities.Models.Collecting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RestSharp;
using RestSharp.Authenticators;
using WebSite.Presta.Clients.Options;
using WebSiteService.Interfaces;
using WebSiteService.Interfaces.Clients;
using WebSiteService.Interfaces.Clients.Responses;

namespace WebSite.Presta.Clients
{
    public class FozzyShopSiteServiceClient : IFozzyShopSiteServiceClient
    {
        private readonly FozzyShopSiteOptions _options;
        private readonly ILogger<IFozzyShopSiteServiceClient> _logger;

        public FozzyShopSiteServiceClient(IOptions<FozzyShopSiteOptions> options, 
            ILogger<IFozzyShopSiteServiceClient> logger)
        {
            _options = options.Value;
            _logger = logger;
        }

        public async Task<PutDataResult> PutOrderData(OrderData orderData, CancellationToken cancellationToken)
        {
            
            var logPrefix = $"FozzyShopSiteServiceClient:PutOrderData";

            _logger.LogInformation("{msg} {data}", $"{logPrefix} started", orderData.GetOrder().orderId);
            try
            {
                var message = orderData.ToXml();

                _logger.LogInformation("{msg} {data}", $"{logPrefix} xml prepared", message);
                var client = new RestClient(_options.BaseUrl);
                client.Timeout = (int)_options.Timeout.TotalMilliseconds;
                client.Authenticator = new HttpBasicAuthenticator(_options.Login, _options.Password);
                var request = new RestRequest("/putordertofozzyshop", Method.POST, DataFormat.Xml)
                    .AddParameter("text/xml", message, ParameterType.RequestBody);

                var response = await client.ExecutePostAsync(request, cancellationToken);

                if(response == null)
                {
                    _logger.LogError("{msg} {error} {data}", $"{logPrefix} null", "response is null", orderData.GetOrder().orderId);

                    return PutDataResult.Error<PutDataResult>(new SiteException(SiteErrors.FormatErrorWebSite, "нет ответа"));
                }

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var result = response.Content.Trim(new char[] { '\uFEFF' }).ToObj<FozzyClientConfirmResponse>("xml");

                    if (result != null && result.errorCode == FozzyClientConfirmResponse.OK)
                    {
                        _logger.LogInformation("{msg} {data}", $"{logPrefix} success", orderData.GetOrder().orderId);

                        return PutDataResult.Success<PutDataResult>(response.StatusCode);
                    }
                    else
                    {
                        _logger.LogError("{msg} {error} {code} {data}", $"{logPrefix} fozzy shop site error", 
                            result?.errorMessage, result?.errorCode, orderData.GetOrder().orderId);

                        return PutDataResult.Error<PutDataResult>(
                            new SiteException(SiteErrors.BusinessErrorWebSite, $"{result?.errorMessage}, code:{result?.errorCode}"));
                    }
                }
                else
                {
                    _logger.LogError("{msg} {error} {statusCode} {data}", $"{logPrefix} fail", response.ErrorMessage, response.StatusCode, 
                        orderData.GetOrder().orderId);

                    return PutDataResult.Error<PutDataResult>(
                        new SiteException(SiteErrors.ErrorWebSite, response.ErrorMessage, response.StatusCode));
                }


            }
            catch (Exception ex)
            {
                _logger.LogError("{msg} {error} {StackTrace} {data}", $"{logPrefix} error", ex.FullMessage(), ex.StackTrace, 
                    orderData.GetOrder().orderId);
                return PutDataResult.Error<PutDataResult>(
                    new SiteException(SiteErrors.UnhandledExceptionWebSite, ex));
            }
        }

    }
}
