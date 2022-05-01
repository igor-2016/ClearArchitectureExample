using ECom.Entities.Models;
using ECom.Types.Delivery;
using ECom.Types.Orders;
using ECom.Types.Requests;
using ECom.Types.TimeSlots;
using ECom.Types.Views;

namespace Basket.Interfaces
{
    public interface IBasketService
    {
        Task<BasketView> CreateOrder(ICreateBasketRequest createBasketRequest,
            IChangeOwnerRequest changeOwnerRequest,
            ISetFilialDeliveryRequest changeFilialDelivery,
            IEnumerable<IAddItemRequest> addItemRequests,
            IPropsCloseRequest propsCloseRequest,
            CancellationToken cancellationToken);

        Task<TimeSlotView> GetTimeSlot(int filialId, DeliveryType deliveryType,
             DateTime onDate, Merchant merchant, CancellationToken cancellationToken);

        Task<FozzyCollectableOrderInfo> GetEComOrderForCollecting(Guid basketId, CancellationToken cancellationToken);

        Task<FozzyCollectableOrderInfo> GetEComOrderAfterAcceptReplacements(Guid basketId, CancellationToken cancellationToken);

        Task<BasketView> GetEComOrder(Guid basketId, CancellationToken cancellationToken);

        Task UpdateCollectingInfo(Guid basketId,
           List<BasketCollectedItem> updatedCollectingItems, CancellationToken cancellationToken);

    }
}