using Collecting.Interfaces;
using Entities.Models.Expansion;
using MediatR;
using Utils.Attributes;

namespace Collecting.UseCases
{
    [ApmTrace]
    public class LoadTraceableOrderCommand : IRequest<TraceableOrder>
    {
        public Guid BasketGuid { get; set; }
    }

    public class LoadTraceableOrderCommandHandler : IRequestHandler<LoadTraceableOrderCommand, TraceableOrder>
    {
        private readonly ICollectingService _collectingService;

        public LoadTraceableOrderCommandHandler(
            ICollectingService dataAccess
            )
        {
            _collectingService = dataAccess;
        }

        public async Task<TraceableOrder> Handle(LoadTraceableOrderCommand request, CancellationToken cancellationToken)
        {
            var basketGuid = request.BasketGuid;

            if (basketGuid == Guid.Empty)
                throw new CollectException(CollectErrors.WrongBasketGuid, "Неверный идентификатор заказа");

            var order = await _collectingService.GetOrderByBasketId(basketGuid, cancellationToken);

            if (order == null)
                throw new CollectException(CollectErrors.OrderNotFound, "Заказ не найден");

            return order;
        }
    }
}
