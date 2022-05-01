using ApplicationServices.Interfaces;
using ApplicationServices.Interfaces.Enums;
using Collecting.Interfaces;
using Collecting.Interfaces.Enums;
using Collecting.Interfaces.Requests;
using Entities.Consts;
using Entities.Models.Expansion;
using MediatR;
using Microsoft.Extensions.Logging;
using Utils;
using Utils.Attributes;

namespace Collecting.UseCases
{
    [ApmTrace]
    public class OfferReplacementsCommand : IRequest<TraceableOrder>
    {
        public ICollectingOnApprovementRequest Request { get; set; }
    }


    public class OfferReplacementsCommandHandler : CollectingCommandHandlerBase,
        IRequestHandler<OfferReplacementsCommand, TraceableOrder>
    {
        private readonly ICollectingService _collectingService;
        private readonly ICommonAppService _app;
        public OfferReplacementsCommandHandler(
            ILogger<OfferReplacementsCommandHandler> logger,
            ICollectingService collectingService,
            ICommonAppService app,
            IMediator mediator) : base (mediator, logger)
        {
            _collectingService = collectingService;
            _app = app;
        }

        public async Task<TraceableOrder> Handle(OfferReplacementsCommand request, CancellationToken cancellationToken)
        {
            var data = request.Request;

            using (_logger.LoggingScope(data.CurrentOrder.BasketId))
            {
                _logger.LogInformation("{msg} {data}", FozzyConsts.CommandInfo.OfferReplacementsCommandName, data);


                // скасоване
                await _mediator.Send(new CheckRefusedOrderCommand()
                {
                    Order = data.CurrentOrder,
                });

                if (data.CurrentOrder.CollectingState != (int)OrderCollectingState.Collecting)
                    throw new CollectException(CollectErrors.CollectingIsNotStarted, Consts.GeneralErrors.CollectingIsNotStarted);

                int oldStateId = data.CurrentOrder.OrderStatus;

                // Update from FozzyShop
                data.CurrentOrder = await _mediator.Send(new UpdateCollectableOrderCommand()
                {
                    OrderToBeUpdated = data.CurrentOrder,
                    SourceOfChanges = data.ChangedOrder,
                });


                data.CurrentOrder.CollectingState = (int)OrderCollectingState.CallCollecting;
                data.CurrentOrder.OrderStatus = (int)FozzyOrderStatus.Status914;

                // сохранить изменения
                await _collectingService.UpdateOrder(data.CurrentOrder, cancellationToken);



                // deadlock don't call UpdateFozzyShopOrder

                try
                {
                    await _app.UpdateCollecting(oldStateId, data.CurrentOrder, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "{msg} {error} {data}", "Update collecting error",
                        $"Update collecting error {ex.Message}",
                        data.CurrentOrder);

                    throw new CollectException(CollectErrors.InternalError, "Ошибка обновления отборки в ECom.Workflow", ex);
                }


                //await _collectOrderStateNotifier.NotifyCollectingStateChanged(new FzShopCollectingOrder(view))

                return data.CurrentOrder;
            }
        }
    }
}
