using ApplicationServices.Interfaces.Requests;
using Collecting.Interfaces;
using Entities.Models.Expansion;
using MediatR;
using Microsoft.Extensions.Logging;
using Utils.Attributes;

namespace Collecting.UseCases
{
    [ApmTrace]
    public class SetPickedQtyCommand : IRequest<TraceableOrder>
    {
        public TraceableOrder Order { get; set; }
        public ISetQtyRequest Request { get; set; }
    }

    public class SetPickedQtyCommandHandler : IRequestHandler<SetPickedQtyCommand, TraceableOrder>
    {
        private readonly ILogger _logger;
        private readonly IMediator _mediator;
       
        public SetPickedQtyCommandHandler(
            ILogger<SetPickedQtyCommandHandler> logger,
            IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        public async Task<TraceableOrder> Handle(SetPickedQtyCommand request, CancellationToken cancellationToken)
        {
            var data = request.Request;
            var order = request.Order;

            _logger.LogInformation("{msg} {data}", FozzyConsts.CommandInfo.SetPickedQtyCommandName, request);

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

           
            //TODO
            //CollectValidationService.ValidateBeforeChangeItem(item);

            await _mediator.Send(new UpdateOrderParamsCommand()
            {
                Order = order
            });

            if(item.PickerQuantity.HasValue)
            {
                item.PickerQuantity = data.ReqAdd ? data.Qty + item.PickerQuantity : data.Qty;
            }
            else
            {
                item.PickerQuantity = data.Qty;
            }

            if (item.PickerQuantity < 0)
                throw new CollectException(CollectErrors.ResultQtyLessZero, "Итоговое кол-во меньше нуля");

            //if (item.MaxQty.HasValue && item.PickerQty > item.MaxQty)
            //    throw new CollectException(CollectErrors.PickedQtyExceedsMaxQtyu, $"Максимально доступное кол-во для этой позиции: {item.MaxQty}");

            //if (item.PickerQty > item.Qty && item.ParentId.HasValue)
            //    throw new CollectException(CollectErrors.WrongQty, "Итоговое кол-во превышает требуемое, для позиции набора");


            //зберігаємо комплектувальника
            item.GlobalUserId = order.GlobalUserId;
            item.PickerName = order.PickerName;
            item.UserInn = order.UserInn;

            //item.SumOut = item.PriceOut * item.PickerQty.Value;
            //item.SumRozn = item.PriceRozn * item.PickerQty.Value;

            // обновление статуса сборки текущего элемента
            //item.ItemState = item.IsCollected()
            //    ? CollectingItemState.Full
            //    : CollectingItemState.Partial;

            await _mediator.Send(new UpdateOrderParamsCommand()
            {
                Order = order
            });

            return order;
        }
    }
}
