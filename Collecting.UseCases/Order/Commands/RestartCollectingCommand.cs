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
    public class RestartCollectingCommand : IRequest<TraceableOrder>
    {
        public ICollectingStartRequest Request { get; set; }
    }

    public class RestartCollectingCommandHandler :
        StartCollectingCommandHandlerBase,
        IRequestHandler<RestartCollectingCommand, TraceableOrder>
    {
        private readonly ICollectingService _collectingService;
        private readonly ICommonAppService _app;
        public RestartCollectingCommandHandler(
             ILogger<RestartCollectingCommandHandler> logger,
            ICollectingService collectingService,
            ICommonAppService app,
            IMediator mediator
            ) : base (mediator, logger)
        {
            _collectingService = collectingService;
            _app = app; 
        }

        public async Task<TraceableOrder> Handle(RestartCollectingCommand request, CancellationToken cancellationToken)
        {
            var data = request.Request;

            using (_logger.LoggingScope(data.CurrentOrder.BasketId))
            {
                _logger.LogInformation("{msg} {data}", FozzyConsts.CommandInfo.RestartCollectingCommandName, data);

                // скасоване
                await _mediator.Send(new CheckRefusedOrderCommand()
                {
                    Order = data.CurrentOrder,
                });

                int oldOrderStatusId = data.CurrentOrder.OrderStatus;

                if (data.CurrentOrder.CollectingState != (int)OrderCollectingState.UpCollecting)
                    throw new CollectException(CollectErrors.CannotStartCollect, Consts.GeneralErrors.NotUpCollectingStatus);

                // новый заказ переведём в состояние "отбирается"
                data.ChangedOrder.CollectingState = (int)OrderCollectingState.Collecting;
                data.ChangedOrder.OrderStatus = (int)FozzyOrderStatus.Status15;

                //await SetPickerId(fozzyOrder, view)

                if (!data.ChangedOrder.CollectStartTime.HasValue)
                {
                    data.ChangedOrder.CollectStartTime = DateTime.Now;
                }

                data.ChangedOrder = await _collectingService.UpdateOrder(data.ChangedOrder, cancellationToken);
                
                // deadlock!!!  don't call UpdateFozzyShopOrder

                try
                {
                    // TODO Polly Retry
                    await _app.StartCollecting(oldOrderStatusId, data.ChangedOrder, cancellationToken); 
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "{msg} {error} {data}", "Restart collecting error",
                        $"Restart collecting error {ex.Message}",
                        data.ChangedOrder);
                }

                //await _collectOrderStateNotifier.NotifyCollectingStateChanged(new FzShopCollectingOrder(view));

                return data.ChangedOrder;
            }
        }

       
    }
}
