using ApplicationServices.Interfaces;
using Collecting.Interfaces;
using Collecting.Interfaces.Requests;
using Entities.Models.Expansion;
using MediatR;
using Microsoft.Extensions.Logging;
using Utils.Attributes;

namespace Collecting.UseCases
{
    [ApmTrace]
    public class DefaultCollectingCommand : IRequest<TraceableOrder>
    {
        public ICollectingAnyRequest Request { get; set; }
    }

    public class DefaultCollectingCommandHandler :
        CollectingCommandHandlerBase,
        IRequestHandler<DefaultCollectingCommand, TraceableOrder>
    {
        private readonly ICollectingService _collectingService;
        private readonly ICommonAppService _app;

        public DefaultCollectingCommandHandler(
            ILogger<DefaultCollectingCommandHandler> logger,
            ICollectingService collectingService,
            ICommonAppService app,
            IMediator mediator
            ): base (mediator, logger)
        {
            _collectingService = collectingService;
            _logger = logger;
            _app = app;
        }

        public async Task<TraceableOrder> Handle(DefaultCollectingCommand request, CancellationToken cancellationToken)
        {
            var data = request.Request;

            _logger.LogInformation("{msg} {data}", FozzyConsts.CommandInfo.DefaultCollectingCommandName, data);

            int oldStateId = data.CurrentOrder.OrderStatus;

            data.CurrentOrder = await _mediator.Send(new ResearchOrderCommand()
            {
                OrderToBeUpdated = data.CurrentOrder,
                SourceOfChanges = data.ChangedOrder,
            });

            data.CurrentOrder.OrderStatus = data.ChangedOrder.OrderStatus;

            try
            {
                await _app.DefaultAction(oldStateId, data.CurrentOrder, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{msg} {error} {data}", "Log error", $"Done collecting error {ex.Message}", data.CurrentOrder);
                throw new CollectException(CollectErrors.InternalError, "Ошибка логирования", ex);
            }

            data.CurrentOrder = await _collectingService.UpdateOrder(data.CurrentOrder, cancellationToken);
            return data.CurrentOrder;
        }
    }
}
