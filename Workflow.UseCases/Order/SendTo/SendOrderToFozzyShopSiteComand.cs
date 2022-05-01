using ApplicationServices.Interfaces;
using Entities.Models.Collecting;
using Entities.Models.Expansion;
using MediatR;
using Microsoft.Extensions.Logging;
using Utils.Attributes;
using WebSiteService.Interfaces;
using Workflow.UseCases.Consts;

namespace Workflow.UseCases.Order
{

    [ApmTrace]
    public class SendOrderToFozzyShopSiteComand : IRequest<OrderData>
    {
        public Guid BasketId { get; set; }

        public int NewStatusId { get; set; }
    }

    public class SendOrderToFozzyShopSiteComandHandler : WorkflowCommandHandlerBase,
        IRequestHandler<SendOrderToFozzyShopSiteComand, OrderData>
    {
        private readonly IWebSiteService _webSiteService;
        private readonly ICommonAppService _commonAppService;

        public SendOrderToFozzyShopSiteComandHandler(
            ILogger<SendOrderToFozzyShopSiteComandHandler> logger,
            IWebSiteService webSiteService,
            ICommonAppService commonAppService,
            IMediator mediator) : base (mediator, logger)
        {
            _webSiteService = webSiteService;
            _commonAppService = commonAppService;
        }

        public async Task<OrderData> Handle(SendOrderToFozzyShopSiteComand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("{msg} {data}", WorkflowConsts.CommandInfo.SendOrderToFozzyShopSiteComandName, request);

            var order = await _mediator.Send(new LoadTraceableOrderCommand()
            {
                BasketGuid = request.BasketId
            }, cancellationToken);

            if (order.OrderStatus != request.NewStatusId)
            {
                order.OrderStatus = request.NewStatusId;
                order = await _commonAppService.UpdateOrderOnly(order, cancellationToken);
            }

           
            return await _webSiteService.SendOrderToFozzyWebSite(order, cancellationToken);
           
        }
    }
}
