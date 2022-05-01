using ApplicationServices.Interfaces.Requests;
using Entities.Models.Expansion;
using MediatR;
using Microsoft.Extensions.Logging;
using Utils;
using Utils.Attributes;
using Workflow.Interfaces.Exceptions;
using Workflow.UseCases.Consts;
using Workflow.UseCases.Requests;

namespace Workflow.UseCases
{
    [ApmTrace]
    public class AddItemCommand : IRequest<TraceableOrder>
    {
        public TraceableOrder Order { get; set; }

        public IAddItemCollectRequest Request { get; set; }

        public bool AllowZeroQty { get; set; } = false;
    }

    public class AddItemCommandHandler : IRequestHandler<AddItemCommand, TraceableOrder>
    {
        private readonly ILogger _logger;
        private readonly IMediator _mediator;

        public AddItemCommandHandler(
            ILogger<AddItemCommandHandler> logger,
            IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        public async Task<TraceableOrder> Handle(AddItemCommand request, CancellationToken cancellationToken)
        {
            var data = request.Request;
            var order = request.Order;

            using (_logger.LoggingScope(order.BasketId))
            {
                _logger.LogInformation("{msg} {data}", WorkflowConsts.CommandInfo.AddItemCommandName, request);
                //await _collectActionLogger.Log(new CollectActionInfoLog(request.OrderId, "AddItem", "Добавление артикула в заказа + количество", request.Context.UserId, lagerId: request.Args.LagerId, barcode: request.Args.Barcode, quantity: request.Args.Qty));


                if (!data.LagerId.HasValue)
                    throw new WorkflowException(WorkflowErrors.LagerIdRequired, "LagerId не найден!");


                var items = await _mediator.Send(new LookupEcomItemCommand()
                {
                   BasketGuid = order.BasketId,
                   Request = new LookupItemRequest
                   {
                      // Barcode = data.Barcode,
                       ItemId = data.EComItemId, // ECom item Id
                       LagerId = data.LagerId
                   }
                });

                if (items.Count == 0)
                    throw new WorkflowException(WorkflowErrors.SKUNotFound, "Товар не найден");

                if (items.Count > 1)
                    throw new WorkflowException(WorkflowErrors.MultipleSKUFound, "найдено более 1 позиции товара");

                var item = items.First();


                if (!data.Qty.HasValue || data.Qty <= 0 || data.Qty > 100000)
                {
                    if(!(request.AllowZeroQty && (data.Qty ?? 0) == 0))
                        throw new WorkflowException(WorkflowErrors.WrongQty, $"неверное количество {data.Qty ?? 0} ({data.LagerId})");
                }

                if (data.Qty.HasValue)
                {
                    await _mediator.Send(new CheckItemQtyCommand()
                    {
                        OrderLine = item,
                        Qty = data.Qty.Value
                    });
                }

                //await _mediator.Send(new UpdateOrderParamsCommand()
                //{
                //    Order = order
                //});

                item.PickerQuantity = data.Qty.Value;

                //item.SumOut = item.PriceOut * item.PickerQty.Value;
                //item.SumRozn = item.PriceRozn * item.PickerQty.Value;

                // replacements
                item.ReplacementOnLagerId = data.ReplacementOnLagerId;
                item.ReplacementLagers = data.Replacements.ToCommaString();

                order.Items.Add(item);

                //await _mediator.Send(new UpdateOrderParamsCommand()
                //{
                //    Order = order
                //});


 
                return order;
            }
        }
    }
}
