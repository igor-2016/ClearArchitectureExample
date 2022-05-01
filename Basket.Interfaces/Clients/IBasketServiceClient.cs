using ECom.Entities.Models;
using ECom.Types.Collect;
using ECom.Types.Delivery;
using ECom.Types.Orders;
using ECom.Types.Requests;
using ECom.Types.TimeSlots;
using ECom.Types.Views;

namespace Basket.Interfaces.Clients
{
    public interface IBasketServiceClient
    {
        Task<BasketView> CreateBasket(ICreateBasketRequest createBasketRequest, CancellationToken cancellationToken);

        Task<BasketView> GetBasket(Guid basketId, CancellationToken cancellationToken);

        Task<BasketView> ChangeOwner(Guid basketId, IChangeOwnerRequest changeOwnerRequest, CancellationToken cancellationToken);

        Task<BasketView> ChangeFilialDelivery(Guid basketId, ISetFilialDeliveryRequest setFilialDeliveryRequest, CancellationToken cancellationToken);
        
        Task<BasketView> CloseWithProps(Guid basketId, IPropsCloseRequest propsCloseRequest, CancellationToken cancellationToken);
        
        Task<BasketView> AddItem(Guid basketId, IAddItemRequest addItemRequest, CancellationToken cancellationToken);

        Task<TimeSlotView> GetTimeSlot(int filialId, DeliveryType deliveryType,
            DateTime onDate, Merchant merchant, CancellationToken cancellationToken);

        Task<FozzyCollectableOrderInfo> GetEComOrderForCollecting(Guid basketId, CancellationToken cancellationToken);

        Task UpdateCollectingInfo(Guid basketId,
            List<BasketCollectedItem> updatedCollectingItems, CancellationToken cancellationToken);
    }
}
