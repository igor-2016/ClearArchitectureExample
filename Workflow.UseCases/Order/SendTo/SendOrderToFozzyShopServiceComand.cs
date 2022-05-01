using ApplicationServices.Interfaces;
using Collecting.Interfaces;
using Entities.Models.Collecting;
using Entities.Models.Expansion;
using MediatR;
using Microsoft.Extensions.Logging;
using Utils.Attributes;
using Workflow.Interfaces.Exceptions;
using Workflow.UseCases.Consts;

namespace Workflow.UseCases.Order
{

    [ApmTrace]
    public class SendOrderToFozzyShopCollectingServiceComand : IRequest<OrderData>
    {
        public Guid BasketId { get; set; }

        public int NewStatusId { get; set; }
    }

    public class SendOrderToFozzyShopCollectingServiceComandHandler :
        WorkflowCommandHandlerBase, IRequestHandler<SendOrderToFozzyShopCollectingServiceComand, OrderData>
        
    {
        private readonly ICollectingService _collectingService;
        private readonly ICommonAppService _commonAppService;

        public SendOrderToFozzyShopCollectingServiceComandHandler(
            ICollectingService collectingService,
            ICommonAppService commonAppService,
            ILogger<SendOrderToFozzyShopCollectingServiceComandHandler> logger,
            IMediator mediator) : base (mediator, logger)
        {
            _collectingService = collectingService;
            _commonAppService = commonAppService;
        }


        public async Task<OrderData> Handle(SendOrderToFozzyShopCollectingServiceComand request, 
            CancellationToken cancellationToken)
        {

            _logger.LogInformation("{msg} {data}", 
                WorkflowConsts.CommandInfo.SendOrderToFozzyShopCollectingServiceComandName, request);

            //var order = await _mediator.Send(new LoadTraceableOrderCommand()
            //{
            //    BasketGuid = request.BasketId,
            //    WithItems = true
            //}, cancellationToken)

            var order = await _commonAppService.GetOrderWithItemsByBasketId(request.BasketId, cancellationToken);
            if (order == null)
                throw new WorkflowException(WorkflowErrors.OrderNotFound);

            if (order.OrderStatus != request.NewStatusId)
            {
                order.OrderStatus = request.NewStatusId;
                order = await _commonAppService.UpdateOrderAndItems(order, cancellationToken);
            }

            return await _collectingService.SendOrderToFozzyWebService(order, cancellationToken);
        }
    }
}
