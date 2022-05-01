using ApplicationServices.Interfaces;
using ApplicationServices.Interfaces.Requests;
using Collecting.UseCases.Requests;
using ECom.Types.Exceptions;
using Entities.Models.Expansion;
using MediatR;
using Microsoft.Extensions.Logging;
using Utils;
using Utils.Attributes;

namespace Collecting.UseCases
{
    [ApmTrace]
    public class UpdateQtyReplacementsCommand : IRequest<TraceableOrder>
    {
        public TraceableOrder OrderToBeUpdated { get; set; } // view

        public TraceableOrder SourceOfChanges { get; set; } // order data

    }

    public class UpdateQtyReplacementsCommandHandler : CollectingCommandHandlerBase,
        IRequestHandler<UpdateQtyReplacementsCommand, TraceableOrder>
    {
        private readonly IFozzyShopAndCollectOrderDifferenceCalculator _сollectOrderDifferenceCalculator;

        public UpdateQtyReplacementsCommandHandler(
            ILogger<UpdateQtyReplacementsCommandHandler> logger,
            IMediator mediator,
            //ICommonAppService app,
            IFozzyShopAndCollectOrderDifferenceCalculator сollectOrderDifferenceCalculator
            ) : base(mediator, logger)
        {
            _сollectOrderDifferenceCalculator = сollectOrderDifferenceCalculator;
        }

        public const bool AllowZeroQty = true;

        public async Task<TraceableOrder> Handle(UpdateQtyReplacementsCommand request, CancellationToken cancellationToken)
        {
            var orderToBeUpdated = request.OrderToBeUpdated;

            var difference = _сollectOrderDifferenceCalculator.Calc(new CollectableOrdersInput()
            {
                SourceOfChanges = request.SourceOfChanges,
                OrderToBeUpdated = orderToBeUpdated,
            });

            var basketId = orderToBeUpdated.BasketId;

            if(difference.ItemsToAdd.Any())
                foreach (var itemToAdd in difference.ItemsToAdd.ToList())
                {
                    try
                    {
                        orderToBeUpdated = await AddItem(orderToBeUpdated, itemToAdd, basketId, AllowZeroQty);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError("{msg} {basketGuid} {error} {StackTrace} {item}", "Невозможно добавить новый товар",
                            basketId, ex.FullMessage(), ex.StackTrace, itemToAdd);

                        throw;
                    }
                }

            // обновим для согласованных записей Qty (не QtyPicked!!!!) и "обнулим" Qty (Qty = QtyPicked) у тех где не собрано.
            var changes = _сollectOrderDifferenceCalculator.CalcChangedQty(new CollectableOrdersInput()
            {
                SourceOfChanges = request.SourceOfChanges,
                OrderToBeUpdated = orderToBeUpdated,
            });

            if (changes.ItemsToChangeQty.Any())
                foreach (var itemToChangeQty in changes.ItemsToChangeQty.ToList())
                {
                    try
                    {
                        _logger.LogError("{msg} {data} {basketGuid}", "Обновление qty", itemToChangeQty.JsonSerialize(), basketId);
                        orderToBeUpdated = await UpdateQtyItem(orderToBeUpdated, itemToChangeQty, basketId);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError("{msg} {basketGuid} {error} {StackTrace} {item}", "Невозможно обновить qty для товара",
                            basketId, ex.FullMessage(), ex.StackTrace, itemToChangeQty);

                        throw;
                    }
                }

            return orderToBeUpdated;
        }

        private Task<TraceableOrder> AddItem(TraceableOrder order, IAddItemCollectRequest request,
            Guid basketGuid, bool allowZeroQty)
        {
            // FozzyShopAddItemCollectRequest
            return _mediator.Send(new AddItemCommand()
            {
                Order = order,
                Request = request,
                AllowZeroQty = allowZeroQty
            }); 
        }

        //it sets qty, not picked qty
        private Task<TraceableOrder> UpdateQtyItem(TraceableOrder order, ISetQtyRequest request, 
            Guid basketGuid)
        {
            // FozzyShopSetQtyRequest
            request.ReqAdd = false;

            return _mediator.Send(new UpdateQtyCommand()
            {
                Order = order,
                Request = request
            });
        }
    }
}
