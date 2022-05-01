using ApplicationServices.Interfaces;
using ApplicationServices.Interfaces.Enums;
using Collecting.Interfaces;
using Collecting.Interfaces.Enums;
using Entities.Models.Expansion;
using MediatR;
using Microsoft.Extensions.Logging;
using Utils;
using Utils.Attributes;
using Workflow.Interfaces.Exceptions;
using Workflow.UseCases.Consts;

namespace Workflow.UseCases.Order
{
    [ApmTrace]
    public class CancelOrderCommand : IRequest<TraceableOrder>
    {
        public Guid BasketGuid { get; set; }
    }

    public class CancelOrderCommandHandler : WorkflowCommandHandlerBase,
        IRequestHandler<CancelOrderCommand, TraceableOrder>
    {
        private readonly ICommonAppService _commonAppService;
        public CancelOrderCommandHandler(
            ICommonAppService commonAppService,
            ILogger<CancelOrderCommandHandler> logger,
            IMediator mediator) : base(mediator, logger)
        {
            _commonAppService = commonAppService;
        }

        public async Task<TraceableOrder> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
        {
            var basketGuid = request.BasketGuid;
            using (_logger.LoggingScope(basketGuid))
            {
                _logger.LogInformation("{msg} {data}", WorkflowConsts.CommandInfo.CancelOrderCommandName, basketGuid);


                var order = await _mediator.Send(new LoadTraceableOrderCommand()
                {
                    BasketGuid = basketGuid,
                    WithItems = false
                }, cancellationToken);



                // скасоване
                await _mediator.Send(new CheckRefusedOrderCommand()
                {
                    Order = order,
                });


                if(order.CollectingState != (int)OrderCollectingState.Collecting)
                {
                    throw new WorkflowException(WorkflowErrors.OrderIsNotInCollecting);
                }

                order.CollectingState = (int)OrderCollectingState.Refused;
                order.OrderStatus = (int)FozzyOrderStatus.Status6;  // отмена

                order = await _commonAppService.UpdateOrderOnly(order, cancellationToken);

                //TODO Workflow notifies site and Collecting service 

                return order;
            }
        }
    }
}
