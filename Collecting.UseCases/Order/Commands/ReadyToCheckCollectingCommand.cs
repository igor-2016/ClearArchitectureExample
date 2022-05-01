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
    public class ReadyToCheckCollectingCommand : IRequest<TraceableOrder>
    {
        public IDefaultCollectingRequest Request { get; set; }
    }

    public class ReadyToCheckCollectingCommandHandler :
        StartCollectingCommandHandlerBase,
        IRequestHandler<ReadyToCheckCollectingCommand, TraceableOrder>
    {
        private readonly ICollectingService _collectingService;

        private readonly ICommonAppService _app;
        public ReadyToCheckCollectingCommandHandler(
            ILogger<ReadyToCheckCollectingCommandHandler> logger,
            ICollectingService collectingService,
            ICommonAppService app,
            IMediator mediator
            ) : base (mediator, logger)
        {
            _collectingService = collectingService;
            _app = app;
        }

        public async Task<TraceableOrder> Handle(ReadyToCheckCollectingCommand request, CancellationToken cancellationToken)
        {
            var data = request.Request;

            using (_logger.LoggingScope(data.ChangedOrder.BasketId))
            {
                _logger.LogInformation("{msg} {data}", FozzyConsts.CommandInfo.ReadyToCheckCollectingCommandName, data);

                // скасоване
                await _mediator.Send(new CheckRefusedOrderCommand()
                {
                    Order = data.ChangedOrder
                });

                if (data.ChangedOrder.OrderStatus != (int)FozzyOrderStatus.Status922)
                {
                    throw new CollectException(CollectErrors.InvalidTsdState);
                }

                if (data.CurrentOrder.OrderStatus != (int)FozzyOrderStatus.Status911)
                {
                    throw new CollectException(CollectErrors.InvalidOrderStatusBeforeReadyToCheckState);
                }

                int oldStateId = data.CurrentOrder.OrderStatus;

                data.ChangedOrder = await _collectingService.UpdateOrder(data.ChangedOrder, cancellationToken);
                
                // deadlock!!!  don't call UpdateFozzyShopOrder

                try
                {
                    await _app.NotifyWorkflowStatusChanged(data.CurrentOrder.BasketId, oldStateId, data.ChangedOrder.OrderStatus, cancellationToken); 
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "{msg} {error} {data}", "Set 922 error", $"Set 922 error {ex.Message}",
                        data.ChangedOrder);

                    throw new CollectException(CollectErrors.ErrorSetReadyToCheckCollectingInWorkflow, ex);
                }

                return data.ChangedOrder;
            }
        }

       
    }
}
