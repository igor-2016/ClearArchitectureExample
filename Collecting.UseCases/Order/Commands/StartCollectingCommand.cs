using ApplicationServices.Interfaces;
using ApplicationServices.Interfaces.Enums;
using Collecting.Interfaces;
using Collecting.Interfaces.Enums;
using Collecting.Interfaces.Requests;
using Entities.Models.Expansion;
using MediatR;
using Microsoft.Extensions.Logging;
using Utils;
using Utils.Attributes;
using Workflow.Interfaces;

namespace Collecting.UseCases
{
    [ApmTrace]
    public class StartCollectingCommand : IRequest<TraceableOrder>
    {
        public ICollectingStartRequest Request { get; set; }
    }

    public class StartCollectingCommandHandler :
        StartCollectingCommandHandlerBase,
        IRequestHandler<StartCollectingCommand, TraceableOrder>
    {
        private readonly ICollectingService _collectingService;
        private readonly IWorkflowService _workflowService;

        private readonly ICommonAppService _app;
        public StartCollectingCommandHandler(
            ILogger<StartCollectingCommandHandler> logger,
            ICollectingService collectingService,
            ICommonAppService app,
            IWorkflowService workflowService,
            IMediator mediator
            ) : base (mediator, logger)
        {
            _collectingService = collectingService;
            _app = app;
            _workflowService = workflowService;
        }

        public async Task<TraceableOrder> Handle(StartCollectingCommand request, CancellationToken cancellationToken)
        {
            var data = request.Request;

            using (_logger.LoggingScope(data.ChangedOrder.BasketId))
            {
                _logger.LogInformation("{msg} {data}", FozzyConsts.CommandInfo.StartCollectingCommandName, data);

                // скасоване
                await _mediator.Send(new CheckRefusedOrderCommand()
                {
                    Order = data.ChangedOrder
                });

                // заказ в состоянии "собирается" просто вернём назад //???
                if (data.CurrentOrder.CollectingState == (int)OrderCollectingState.Collecting)
                {
                    return data.CurrentOrder;
                }

                if (data.CurrentOrder.CollectingState != (int)OrderCollectingState.New)
                    throw new CollectException(CollectErrors.CannotStartCollect, "Невозможно начать отборку этого заказа");
                
                int oldStateId = data.CurrentOrder.OrderStatus;


                // новый заказ переведём в состояние "отбирается"
                data.ChangedOrder.CollectingState = (int)OrderCollectingState.Collecting;
                data.ChangedOrder.OrderStatus = (int)FozzyOrderStatus.Status15;

                
                if (!data.ChangedOrder.CollectStartTime.HasValue)
                {
                    data.ChangedOrder.CollectStartTime = DateTime.Now;
                }

                data.ChangedOrder = await _collectingService.UpdateOrder(data.ChangedOrder, cancellationToken);
                
                // deadlock!!!  don't call UpdateFozzyShopOrder

                try
                {
                    await _app.StartCollecting(oldStateId, data.ChangedOrder, cancellationToken); 
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "{msg} {error} {data}", "Start collecting error",
                        $"Start collecting error {ex.Message}",
                        data.ChangedOrder);
                    throw new CollectException(CollectErrors.InternalError, "Ошибка старта отборки в ECom.Workflow", ex);
                }

                //await _collectOrderStateNotifier.NotifyCollectingStateChanged(new FzShopCollectingOrder(view))

                return data.ChangedOrder;
            }
        }

       
    }
}
