using ApplicationServices.Interfaces.Requests;
using Collecting.Interfaces;
using Entities.Models.Expansion;
using MediatR;
using Microsoft.Extensions.Logging;
using Utils;
using Utils.Attributes;

namespace Collecting.UseCases
{
    [ApmTrace]
    public class RemoveItemCommand : IRequest<TraceableOrder>
    {
        public TraceableOrder Order { get; set; }
        public IRemoveItemRequest Request { get; set; }
    }

    public class RemoveItemCommandHandler : IRequestHandler<RemoveItemCommand, TraceableOrder>
    {
        private readonly ILogger _logger;
        private readonly IMediator _mediator;

        public RemoveItemCommandHandler(
            ILogger<RemoveItemCommandHandler> logger,
            IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        public async Task<TraceableOrder> Handle(RemoveItemCommand request, CancellationToken cancellationToken)
        {
            var data = request.Request;
            var order = request.Order;

            using (_logger.LoggingScope(order.BasketId))
            {
                _logger.LogInformation("{msg} {data}", FozzyConsts.CommandInfo.RemoveItemCommandName, request);

                // Обработка параметров
                var list = data.Items.EmptyIfNull().ToList();
                if (data.ItemId.HasValue) list.Add(data.ItemId.Value);

                // Валидации
                var checkAllItems =
                    from li in list
                    from oi in order.Items.Where(oi => oi.Id == li).DefaultIfEmpty()
                    where (oi == null)
                    select li;

                //var checkChildItems =
                //    from li in list
                //    from oi in order.Items
                //    where (oi.Id == li) && (oi.ParentId.HasValue)
                //    select oi;

                if (checkAllItems.Any())
                    throw new CollectException(CollectErrors.PositionNotFound, "Строка для удаления не найдена");

                //if (checkChildItems.Any())
                //    throw new CollectException(CollectErrors.ProductSetItemCannotBeDeleted, "Нельзя удалять элемент продуктового набора");


                // Список позиций для удаления
                var itemsToRemove =
                    from li in list
                    from oi in order.Items.EmptyIfNull()
                    where (oi.Id == li)
                    select oi;

                //var regularItems = itemsToRemove;//.Where(x => !x.IsSet);
                //var productSets = itemsToRemove.Where(x => x.IsSet);


                //order.Items.RemoveAll(x => regularItems.Contains(x));
                //foreach (var current in productSets)
                //{                 
                //    current.Qty = 0;
                //    current.PickerQty = 0;
                //    current.ItemState = CollectingItemState.Removed;
                //    foreach (var child in current.Items.EmptyIfNull())
                //    {
                //        child.PickerQty = 0;
                //    }
                //}


                await _mediator.Send(new UpdateOrderParamsCommand()
                {
                    Order = order
                });

                return order;
            }
        }
    }
}
