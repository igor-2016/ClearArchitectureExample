using ApplicationServices.Interfaces.Enums;
using Entities.Models.Expansion;
using MediatR;
using Workflow.Interfaces.Exceptions;

namespace Workflow.UseCases.Order
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
                throw new WorkflowException(WorkflowErrors.OrderRefused, $"Замовлення №{order.OrderNumber} скасоване.");

            await Task.CompletedTask;
        }
    }
}
