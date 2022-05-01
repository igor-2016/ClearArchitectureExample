using Entities.Models.Expansion;
using MediatR;
using Utils.Attributes;

namespace Collecting.UseCases
{
    [ApmTrace]
    public class UpdateOrderParamsCommand : IRequest
    {
        public TraceableOrder Order { get; set; }
    }

    public class UpdateOrderParamsCommandHandler : AsyncRequestHandler<UpdateOrderParamsCommand>
    {
  
        public UpdateOrderParamsCommandHandler()
        {
        }

        protected override async Task Handle(UpdateOrderParamsCommand request, CancellationToken cancellationToken)
        {
            var order = request.Order;

            //var collectedItems = order.Items.Where(x => !x.IsSet)
            //    .Where(x => x.PickerQty.HasValue && x.PickerQty.Value > 0 && x.Collectable);

            //var collectedItemsWithWeight = collectedItems
            //    .Where(x => x.WeightBrutto.HasValue);

            ////считаем вес фактически собранных позиций
            //order.TotalWeight = collectedItemsWithWeight
            //    .Sum(x => x.WeightBrutto.Value * x.PickerQty.Value);

            //var lst = collectedItemsWithWeight
            //    .Select(x => x.WeightBrutto.Value).ToList();

            //order.MaxWeight = (lst.Count > 0) ? lst.Max() : 0;

            //order.SumOut = collectedItems.Sum(x => x.SumOut);

            await Task.CompletedTask;
        }
    }
}
