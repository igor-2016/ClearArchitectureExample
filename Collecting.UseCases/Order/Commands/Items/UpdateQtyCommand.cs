using ApplicationServices.Interfaces.Requests;
using Collecting.Interfaces;
using Entities.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using Utils.Attributes;
using Entities.Models.Expansion;

namespace Collecting.UseCases
{
    [ApmTrace]
    public class UpdateQtyCommand : IRequest<TraceableOrder>
    {
        public TraceableOrder Order { get; set; }
        public ISetQtyRequest Request { get; set; }
    }

    public class UpdateQtyCommandHandler : IRequestHandler<UpdateQtyCommand, TraceableOrder>
    {
        private readonly ILogger _logger;
        private readonly IMediator _mediator;

        public UpdateQtyCommandHandler(
            ILogger<UpdateQtyCommandHandler> logger,
            IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        public async Task<TraceableOrder> Handle(UpdateQtyCommand request, CancellationToken cancellationToken)
        {
            var data = request.Request;
            var order = request.Order;

            _logger.LogInformation("{msg} {data}", FozzyConsts.CommandInfo.UpdateQtyCommandName, request);

            var item = order.Items
                .EmptyIfNull()
                .FirstOrDefault(x => x.Id == data.ItemId);

            if (item == null)
                throw new CollectException(CollectErrors.InternalError, "Not found item.");

            await _mediator.Send(new CheckItemQtyCommand()
            {
                OrderLine = item,
                Qty = data.Qty
            });


            item.OrderQuantity = data.ReqAdd ? data.Qty + item.OrderQuantity : data.Qty;


            if (item.OrderQuantity < 0)
                throw new CollectException(CollectErrors.ResultQtyLessZero, "Итоговое кол-во меньше нуля");

            //if (item.MaxQty.HasValue && item.Qty > item.MaxQty)
            //    throw new CollectException(CollectErrors.PickedQtyExceedsMaxQtyu, $"Максимально доступное кол-во для этой позиции: {item.MaxQty}");

            var pickedQty = (item.PickerQuantity ?? 0);
            if (pickedQty > 0 && item.OrderQuantity < pickedQty)
            {
                // После согласования теперь можем понизить количество ниже собранного
                item.PickerQuantity = item.OrderQuantity;
                // throw new CollectException(CollectErrors.WrongQty, "Невозможно указать количество меньше собранного");
            }

            return order;
        }
    }
}
