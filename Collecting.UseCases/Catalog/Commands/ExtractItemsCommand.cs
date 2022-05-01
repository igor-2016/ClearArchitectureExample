using Collecting.Interfaces;
using ECom.Entities.Models;
using Entities.Models.Expansion;
using MediatR;

namespace Collecting.UseCases
{
    public class ExtractItemsCommand : IRequest<List<TraceableOrderItem>>
    {
        public TraceableOrder Order { get; set; }
        public CatalogItem? CatalogItem { get; set; }

        //public Guid? EComItemId { get; set; }
    }

    public class ExtractItemsCommandHandler : IRequestHandler<ExtractItemsCommand, List<TraceableOrderItem>>
    {
        public ExtractItemsCommandHandler()
        {
        }

        public Task<List<TraceableOrderItem>> Handle(ExtractItemsCommand request, CancellationToken cancellationToken)
        {

            //if (request.CatalogItem == null && !request.EComItemId.HasValue)
            //    throw new CollectException(CollectErrors.NotFoundCatalogItem, "Товар не указан!");

            //if (request.CatalogItem != null && request.EComItemId.HasValue)
            //    throw new CollectException(CollectErrors.InvalidUsageSearchOrderItem);

            if (request.Order == null)
                throw new CollectException(CollectErrors.OrderNotFound, "Ордер не найден!");

            //if (request.EComItemId.HasValue)
            //{
            //    var ecomItemId = request.EComItemId.Value;
            //    return Task.FromResult(request.Order.Items.Where(x => x.EcomItemId == ecomItemId).ToList());
            //}
            //else
            //{
                var lagerId = request.CatalogItem.Id;
                return Task.FromResult(request.Order.Items.Where(x => x.LagerId == lagerId).ToList());
            //}
        }
    }
}
