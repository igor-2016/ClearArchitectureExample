using ApplicationServices.Interfaces;
using Collecting.Interfaces;
using ECom.Entities.Models;
using Entities.Models.Expansion;

namespace ApplicationServices.Implementation.EventHandlers
{
    public class ExtractData
    {
        public CatalogItem? CatalogItem { get; }

        public Guid? EComItemId { get; }

        public ExtractData(CatalogItem? catalogItem, Guid? eComItemId)
        {
            if (catalogItem == null && !eComItemId.HasValue)
                throw new CollectException(CollectErrors.InvalidUsageSearchOrderItem);

            if (catalogItem != null && eComItemId.HasValue)
                throw new CollectException(CollectErrors.InvalidUsageSearchOrderItem);

            CatalogItem = catalogItem;
            EComItemId = eComItemId;
        }
    }

    public class ExtractItemsEvent : IOrderProcessEvent<ExtractData>
    {
        public TraceableOrder Order { get; set; }
        public ExtractData Data { get; set; }
    }

    public class ExtractItemsByLagerIdOrEComItemIdHandler 
    {
        public Task<List<TraceableOrderItem>> Handle(ExtractItemsEvent request, 
            CancellationToken cancellationToken = default)
        {

            if (request.Order == null)
                throw new CollectException(CollectErrors.OrderNotFound, "Ордер не найден!");

            //if (request.Data.EComItemId.HasValue)
            //{
            //    var ecomItemId = request.Data.EComItemId.Value;
            //    return Task.FromResult(request.Order.Items.Where(x => x.EcomItemId == ecomItemId).ToList());
            //}
            //else 
            
            if(request.Data.CatalogItem != null)
            {
                var lagerId = request.Data.CatalogItem.Id;
                return Task.FromResult(request.Order.Items.Where(x => x.LagerId == lagerId).ToList());
            }
            else
            {
               throw new CollectException(CollectErrors.InvalidUsageSearchOrderItem);
            }
        }
    }
}
