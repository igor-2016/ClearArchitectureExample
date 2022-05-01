using ApplicationServices.Interfaces.Requests;
using Catalog.Interfaces;
using Collecting.Interfaces;
using Collecting.UseCases.Requests;
using Entities.Models.Expansion;
using MediatR;
using Microsoft.Extensions.Logging;
using Utils;
using Utils.Attributes;

namespace Collecting.UseCases
{


    // TODO retest !!! чем отличается от add item ???
    [ApmTrace]
    public class AddItemFromReplacementsCommand : IRequest<TraceableOrder>
    {
        public TraceableOrder Order { get; set; }
        public IAddItemCollectRequest Request { get; set; }
    }

    public class AddItemFromReplacementsCommandHandler : IRequestHandler<AddItemFromReplacementsCommand, TraceableOrder>
    {
        private readonly ILogger _logger;
        private readonly IMediator _mediator;
        private readonly ICatalogService _catalogService;

        public AddItemFromReplacementsCommandHandler(
            ILogger<AddItemFromReplacementsCommandHandler> logger,
            ICatalogService catalogService,
            IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
            _catalogService = catalogService;   
        }

        public async Task<TraceableOrder> Handle(AddItemFromReplacementsCommand request, CancellationToken cancellationToken)
        {
            var data = request.Request;
            var order = request.Order;

            using (_logger.LoggingScope(order.BasketId))
            {
                _logger.LogInformation("{msg} {data}", FozzyConsts.CommandInfo.AddItemFromReplacementsCommandName, request);


                if (!data.LagerId.HasValue)
                    throw new CollectException(CollectErrors.BarcodeOrLagerIdRequired, "LagerId не найден!");

               

                var items = await _mediator.Send(new LookupItemCommand()
                {
                    Request = new LookupItemRequest
                    {
                        LagerId = data.LagerId,
                    }
                });

                if (items.Count == 0)
                    throw new CollectException(CollectErrors.SKUNotFound, "Товар не найден");

                if (items.Count > 1)
                    throw new CollectException(CollectErrors.MultipleSKUFound, "найдено более 1 позиции товара");

                var item = items.First();

                if (data.Qty.HasValue)
                {
                    await _mediator.Send(new CheckItemQtyCommand()
                    {
                        OrderLine = item,
                        Qty = data.Qty.Value
                    });
                }

                //if (data.Args.Qty.Value > 1 && item.IsSet)
                //    throw new CollectException(CollectErrors.WrongQty, "Добавление наборов в заказ, выполняется по 1 экземпляру.");

                
                await _mediator.Send(new UpdateOrderParamsCommand()
                {
                    Order = order
                });

                item.PickerQuantity = data.Qty.Value;

                //item.SumOut = item.PriceOut * item.PickerQty.Value;
                //item.SumRozn = item.PriceRozn * item.PickerQty.Value;

                // replacements
                item.ReplacementOnLagerId = data.ReplacementOnLagerId;
                item.ReplacementLagers = data.Replacements.ToCommaString();

                order.Items.Add(item);

                await _mediator.Send(new UpdateOrderParamsCommand()
                {
                    Order = order
                });


                _logger.LogInformation("{msg} {data}", FozzyConsts.CommandInfo.AddItemFromReplacementsCommandName + " After",
                    item.ReplacementLagers);

                return order;
            }
        }
    }
}
