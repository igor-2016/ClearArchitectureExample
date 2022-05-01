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
    public class DoneCollectingCommand : IRequest<TraceableOrder>
    {
        public ICollectingDoneRequest Request { get; set; }
    }

    public class DoneCollectingCommandHandler : CollectingCommandHandlerBase,
        IRequestHandler<DoneCollectingCommand, TraceableOrder>
    {
        private readonly ICollectingService _collectingService;
        private readonly ICommonAppService _app;
        public DoneCollectingCommandHandler(
            ILogger<DoneCollectingCommandHandler> logger,
            ICollectingService collectingService,
            ICommonAppService app,
            IMediator mediator) : base (mediator, logger)
        {
            _collectingService = collectingService;
            _app = app;
        }

        public async Task<TraceableOrder> Handle(DoneCollectingCommand request, CancellationToken cancellationToken)
        {
            var data = request.Request;

            using (_logger.LoggingScope(data.CurrentOrder.BasketId))
            {
                _logger.LogInformation("{msg} {data}", FozzyConsts.CommandInfo.DoneCollectingCommandName, data);

                // скасоване
                await _mediator.Send(new CheckRefusedOrderCommand()
                {
                    Order = data.CurrentOrder,
                });


                if (data.CurrentOrder.CollectingState != (int)OrderCollectingState.Collecting)
                    throw new CollectException(CollectErrors.CollectingIsNotStarted, Consts.GeneralErrors.CollectingIsNotStarted);

                int oldStateId = data.CurrentOrder.OrderStatus;

                //Update from FozzyShop
                data.CurrentOrder = await _mediator.Send(new DoneCollectableOrderCommand()
                {
                    OrderToBeUpdated = data.CurrentOrder,
                    SourceOfChanges = data.ChangedOrder,
                });

                // закрыть заказ
                data.CurrentOrder.CollectingState = (int)OrderCollectingState.Collected;
                data.CurrentOrder.OrderStatus = (int)FozzyOrderStatus.Status911;

                try
                {
                    await _app.DoneCollecting(oldStateId, data.CurrentOrder, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "{msg} {error} {data}", "Done collecting error",
                        $"Done collecting error {ex.Message}",
                        data.CurrentOrder);

                    throw new CollectException(CollectErrors.InternalError, "Ошибка завершения отборки в ECom.Workflow", ex);
                }

               
                data.CurrentOrder.CollectEndTime = DateTime.Now;
                data.CurrentOrder.DateModified = DateTime.Now;


                // сохранить изменения
                data.CurrentOrder = await _collectingService.UpdateOrder(data.CurrentOrder, cancellationToken);

                // deadlock don't call UpdateFozzyShopOrder

                return data.CurrentOrder;
            }
        }
    }
}
