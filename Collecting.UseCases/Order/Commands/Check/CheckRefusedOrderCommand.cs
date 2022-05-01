using ApplicationServices.Interfaces.Enums;
using Collecting.Interfaces;
using Entities.Models.Expansion;
using MediatR;

namespace Collecting.UseCases
{
    public class CheckRefusedOrderCommand : IRequest
    {
        public TraceableOrder Order { get; set; }
    }

    public class CheckRefusedOrderCommandHandler : AsyncRequestHandler<CheckRefusedOrderCommand>
    {
     
        public CheckRefusedOrderCommandHandler()
        {
        }

        protected override async Task Handle(CheckRefusedOrderCommand request, CancellationToken cancellationToken)
        {
            var order = request.Order;

            if (order.CollectingState == (int)OrderCollectingState.Refused)
                throw new CollectException(CollectErrors.OrderRefused, $"Замовлення №{order.OrderNumber} скасоване.");

            await Task.CompletedTask;
        }
    }
}
