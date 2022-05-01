using Basket.Interfaces;
using Basket.Interfaces.Clients;
using ECom.Entities.Models;
using ECom.Types.Delivery;
using ECom.Types.Orders;
using ECom.Types.Requests;
using ECom.Types.ServiceBus;
using ECom.Types.TimeSlots;
using ECom.Types.Views;
using Microsoft.Extensions.Logging;
using Utils.Consts;
using Utils.Extensions;
using Utils.Sys.Exceptions;

namespace Basket.Ecom
{
    public class BasketService : IBasketService
    {
        private readonly IBasketServiceClient _basketServiceClient;
        private readonly ILogger<BasketService> _logger;

        public BasketService(
            IBasketServiceClient basketServiceClient,
            ILogger<BasketService> logger)
        {
            _basketServiceClient = basketServiceClient;
            _logger = logger;
        }

        public async Task<BasketView> CreateOrder(
            ICreateBasketRequest createBasketRequest,
            IChangeOwnerRequest changeOwnerRequest,
            ISetFilialDeliveryRequest changeFilialDelivery,
            IEnumerable<IAddItemRequest> addItemRequests,
            IPropsCloseRequest propsCloseRequest,
            CancellationToken cancellationToken)
        {
            try
            {
                var view = await _basketServiceClient.CreateBasket(createBasketRequest, cancellationToken);
                var basketId = view.Id;
                await _basketServiceClient.ChangeOwner(basketId, changeOwnerRequest, cancellationToken);
                await _basketServiceClient.ChangeFilialDelivery(basketId, changeFilialDelivery, cancellationToken);
                foreach (var item in addItemRequests)
                {
                    await _basketServiceClient.AddItem(basketId, item, cancellationToken);
                }

                return await _basketServiceClient.CloseWithProps(basketId, propsCloseRequest, cancellationToken);
            }
            catch (PresentationException<EComError> ex)
            {
                throw SetTarget(ex);
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public async Task<TimeSlotView> GetTimeSlot(int filialId, DeliveryType deliveryType,
             DateTime onDate, Merchant merchant, CancellationToken cancellationToken)
        {
            try
            {
                var times = 0; var maxTimes = 12;
                TimeSlotView timeSlot;
                while (true)
                {
                    timeSlot = await _basketServiceClient.GetTimeSlot(filialId, deliveryType, onDate, merchant, cancellationToken);

                    if (timeSlot.IsAvailable)
                        break;

                    times++;

                    if (times > maxTimes)
                        break;

                    onDate = onDate.AddHours(1);
                }

                if (!timeSlot.IsAvailable)
                    throw new BasketException(BasketErrors.NotFoundTimeSlot);

                return timeSlot;

            }
            catch (PresentationException<EComError> ex)
            {
                throw SetTarget(ex);
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public async Task<FozzyCollectableOrderInfo> GetEComOrderForCollecting(Guid basketId, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _basketServiceClient.GetEComOrderForCollecting(basketId, cancellationToken);
                if(result != null)
                {
                    result.CourierName  = result.CourierName ?? string.Empty;
                    result.Notes = result.Notes ?? string.Empty;
                    result.ContragentFullName = result.ContragentFullName ?? string.Empty;
                    result.ContragentOKPO = result.ContragentOKPO ?? string.Empty;
                    result.CustomerName = result.CustomerName ?? string.Empty;  
                    result.CustomerPhone = result.CustomerPhone ?? string.Empty;    
                }
                return result; 
            }
            catch (PresentationException<EComError> ex)
            {
                throw SetTarget(ex);
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public async Task<BasketView> GetEComOrder(Guid basketId, CancellationToken cancellationToken)
        {
            try
            {
                return await _basketServiceClient.GetBasket(basketId, cancellationToken);
            }
            catch (PresentationException<EComError> ex)
            {
                throw SetTarget(ex);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task UpdateCollectingInfo(Guid basketId,
           List<BasketCollectedItem> updatedCollectingItems, CancellationToken cancellationToken)
        {
            try
            {
                await _basketServiceClient.UpdateCollectingInfo(basketId, updatedCollectingItems, cancellationToken);
            }
            catch (PresentationException<EComError> ex)
            {
                throw SetTarget(ex);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<FozzyCollectableOrderInfo> GetEComOrderAfterAcceptReplacements(Guid basketId, CancellationToken cancellationToken)
        {
            try
            {
                return await _basketServiceClient.GetEComOrderForCollecting(basketId, cancellationToken);
            }
            catch (PresentationException<EComError> ex)
            {
                throw SetTarget(ex);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private Exception SetTarget(PresentationException<EComError> ex)
        {
            return new BasketException(BasketErrors.CreateBasketOrderError, ex.ErrorModel?.ErrorMessage ?? ex.Message, ex.Message)
                    .AddRequestedTarget(CommonConsts.Subsystem.ECom);
        }

        
    }
}