using Basket.Interfaces.Clients;
using Basket.Interfaces.Clients.Options;
using ECom.Entities.Models;
using ECom.Types.Delivery;
using ECom.Types.Orders;
using ECom.Types.Requests;
using ECom.Types.ServiceBus;
using ECom.Types.TimeSlots;
using ECom.Types.Views;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using System.Globalization;
using Utils.Sys.RichHttpClient.Extensions;
using static Utils.Sys.RichHttpClient.Extensions.HttpClientExtensions;

namespace Basket.Ecom.Clients
{

    public class BasketApiClient : IBasketServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly BasketServiceOptions _options;
        private readonly ILogger<BasketApiClient> _logger;

        public BasketApiClient(HttpClient httpClient, IOptions<BasketServiceOptions> options, ILogger<BasketApiClient> logger)
        {
            _httpClient = httpClient;
            _options = options.Value;
            _logger = logger;
        }

        public async Task<BasketView> CreateBasket(ICreateBasketRequest createBasketRequest, CancellationToken cancellationToken)
        {
            return await GetDefaultPolicy(nameof(CreateBasket)).ExecuteAsync(
                () => _httpClient.SafePostAsync<ICreateBasketRequest, BasketView, EComError>(
                    _options.CreateBasketMethod, createBasketRequest, cancellationToken));
        }


        


        public async Task<BasketView> ChangeOwner(Guid basketId, IChangeOwnerRequest changeOwnerRequest, CancellationToken cancellationToken)
        {
            return await GetDefaultPolicy(nameof(ChangeOwner)).ExecuteAsync(
              () => _httpClient.SafePostAsync<IChangeOwnerRequest, BasketView, EComError>(
                string.Format(_options.ChangeOwnerMethodFormat, basketId), changeOwnerRequest, cancellationToken));
        }

        public async Task<BasketView> ChangeFilialDelivery(Guid basketId, ISetFilialDeliveryRequest setFilialDeliveryRequest, CancellationToken cancellationToken)
        {
            return await GetDefaultPolicy(nameof(ChangeFilialDelivery)).ExecuteAsync(
              () => _httpClient.SafePostAsync<ISetFilialDeliveryRequest, BasketView, EComError>(
                 string.Format(_options.ChangeFilialMethodFormat, basketId), setFilialDeliveryRequest, cancellationToken));
        }

        public async Task<BasketView> GetBasket(Guid basketId,  CancellationToken cancellationToken)
        {
            return await _httpClient.SafeGetAsync<BasketView, EComError>(
                string.Format(_options.GetBasketMethodFormat, basketId), cancellationToken);
        }

        public async Task<BasketView> AddItem(Guid basketId, IAddItemRequest addItemRequest, CancellationToken cancellationToken)
        {
            return await GetDefaultPolicy(nameof(AddItem)).ExecuteAsync(() => _httpClient.SafePostAsync<IAddItemRequest, BasketView, EComError>(
                string.Format(_options.AddItemMethodFormat, basketId), addItemRequest, cancellationToken));
        }
        
        public async Task<BasketView> CloseWithProps(Guid basketId, IPropsCloseRequest propsCloseRequest, CancellationToken cancellationToken)
        {
            return await GetDefaultPolicy(nameof(CloseWithProps)).ExecuteAsync(() => _httpClient.SafePostAsync<IPropsCloseRequest, BasketView, EComError>(
                string.Format(_options.CloseBasketMethodFormat, basketId), propsCloseRequest, cancellationToken));
        }
        
       
        public async Task<TimeSlotView> GetTimeSlot(int filialId, DeliveryType deliveryType,
             DateTime onDate, Merchant merchant, CancellationToken cancellationToken)
        {
            var query = string.Format(_options.GetTimeSlotMethodFormat, $"filialId={filialId}&deliveryType={(int)deliveryType}&dateFrom={onDate.ToString(new CultureInfo("en"))}&merchant={(int)merchant}");
            return await _httpClient.SafeGetAsync<TimeSlotView, EComError>(query, cancellationToken);
        }


        public async Task<FozzyCollectableOrderInfo> GetEComOrderForCollecting(Guid basketId, CancellationToken cancellationToken)
        {
            var query = string.Format(_options.GetCollactableOrderMethodFormat, basketId);
            return await _httpClient.SafeGetAsync<FozzyCollectableOrderInfo, EComError>(query, cancellationToken);
        }
        
        public async Task UpdateCollectingInfo(Guid basketId, 
            List<BasketCollectedItem> updatedCollectingItems, CancellationToken cancellationToken)
        {
            var query = string.Format(_options.UpdateCollectingMethodFormat, basketId);
            await GetDefaultPolicy(nameof(UpdateCollectingInfo)).ExecuteAsync(() => _httpClient.SafePostAsync<List<BasketCollectedItem>, Unit, EComError>(query, 
                updatedCollectingItems, cancellationToken));
        }


        private AsyncPolicy GetDefaultPolicy(string policyName)
        {
            return Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(3, retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt) / 2), onRetry: (result, time) =>
                    {
                        _logger.LogWarning("{msg} {error} {time}", policyName, result?.Message, time);
                    });
        }
    }

}
