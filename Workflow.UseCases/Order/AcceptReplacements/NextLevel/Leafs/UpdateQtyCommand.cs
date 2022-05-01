using ApplicationServices.Interfaces.Requests;
using Collecting.Interfaces;
using Entities.Models.Expansion;
using MediatR;
using Microsoft.Extensions.Logging;
using Utils.Attributes;
using Workflow.Interfaces.Exceptions;
using Workflow.UseCases.Consts;

namespace Workflow.UseCases
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

            _logger.LogInformation("{msg} {data}", WorkflowConsts.CommandInfo.UpdateQtyCommandName, request);

            var item = order.Items
                .EmptyIfNull()
                .FirstOrDefault(x => x.Id == data.ItemId);

            if (item == null)
                throw new WorkflowException(WorkflowErrors.NotFoundItem, "Not found item.");

            await _mediator.Send(new CheckItemQtyCommand()
            {
                OrderLine = item,
                Qty = data.Qty
            });


            item.OrderQuantity = data.ReqAdd ? data.Qty + item.OrderQuantity : data.Qty;


            if (item.OrderQuantity < 0)
                throw new WorkflowException(WorkflowErrors.ResultQtyLessZero, "Итоговое кол-во меньше нуля");

            var pickedQty = (item.PickerQuantity ?? 0);
            if (pickedQty > 0 && item.OrderQuantity < pickedQty)
            {
                // После согласования теперь можем понизить количество ниже собранного
                item.PickerQuantity = item.OrderQuantity;
                // throw new WorkflowException(WorkflowErrors.WrongQty, "Невозможно указать количество меньше собранного");
            }

            return order;
        }
    }
}
