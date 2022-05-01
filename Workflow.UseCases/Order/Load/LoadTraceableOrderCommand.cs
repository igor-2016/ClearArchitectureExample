using ApplicationServices.Interfaces;
using Collecting.Interfaces;
using Entities.Models.Expansion;
using MediatR;
using Utils.Attributes;
using Workflow.Interfaces;
using Workflow.Interfaces.Exceptions;

namespace Workflow.UseCases.Order
{
    [ApmTrace]
    public class LoadTraceableOrderCommand : IRequest<TraceableOrder>
    {
        public Guid BasketGuid { get; set; }

        public bool WithItems { get; set; } = true; 
    }

    public class LoadTraceableOrderCommandHandler : IRequestHandler<LoadTraceableOrderCommand, TraceableOrder>
    {
        private readonly ICommonAppService _commonAppService;

        public LoadTraceableOrderCommandHandler(
            ICommonAppService commonAppService
            )
        {
            _commonAppService = commonAppService;
        }

        public async Task<TraceableOrder> Handle(LoadTraceableOrderCommand request, CancellationToken cancellationToken)
        {
            var basketGuid = request.BasketGuid;

            if (basketGuid == Guid.Empty)
                throw new WorkflowException(WorkflowErrors.WrongBasketGuid, "Неверный идентификатор заказа");

            var order = request.WithItems
                ? await _commonAppService.GetOrderWithItemsByBasketId(basketGuid, cancellationToken)
                : await _commonAppService.GetOrderOnlyByBasketId(basketGuid, cancellationToken);

            if (order == null)
                throw new WorkflowException(WorkflowErrors.OrderNotFound, "Заказ не найден");

            return order;
        }
    }
}
