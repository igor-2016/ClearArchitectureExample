using ApplicationServices.Interfaces.Requests;
using Catalog.Interfaces.Dto;
using Entities.Models.Expansion;
using MediatR;

namespace Collecting.UseCases
{
    public class ExtractItemsCommand : IRequest<List<TraceableOrderItem>>
    {
        public TraceableOrder Order { get; set; }
        public CatalogResponse GoodsDataRequest { get; set; }
        public ILookupItemRequest Request { get; set; }
    }

    public class ExtractItemsCommandHandler : IRequestHandler<ExtractItemsCommand, List<TraceableOrderItem>>
    {
        public ExtractItemsCommandHandler()
        {
        }

        public async Task<List<TraceableOrderItem>> Handle(ExtractItemsCommand request, CancellationToken cancellationToken)
        {
            //var hasItemId = request.Request.ItemId.HasValue;
            var lagerId = request.GoodsDataRequest.GoodsData.LagerId;
            //var isSet = request.GoodsDataRequest.GoodsData.Items.EmptyIfNull().Any();

            var plain = request.Order.Items.ToList();
            var items =
                from c in plain
                    //from p in plain.Where(p => p.Id == c.ParentId).DefaultIfEmpty()
                    //where p == null || p.Qty > 0
                    //where !hasItemId && c.ParentId == null || hasItemId && c.ParentId == request.Request.ItemId.Value
                //where hasItemId
                where c.LagerId == lagerId
                //where !isSet
                select c;

            await Task.CompletedTask;

            return items.ToList();
        }
    }
}
