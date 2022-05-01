using Collecting.Interfaces;
using ECom.Entities.Models;
using Entities.Models.Expansion;
using MediatR;

namespace Workflow.UseCases
{
    public class ExtractItemsByEcomIdCommand : IRequest<List<TraceableOrderItem>>
    {
        public TraceableOrder Order { get; set; }
     
        public Guid EComItemId { get; set; }
    }

    public class ExtractItemsByEcomIdCommandHandler : IRequestHandler<ExtractItemsByEcomIdCommand, List<TraceableOrderItem>>
    {
        

        public Task<List<TraceableOrderItem>> Handle(ExtractItemsByEcomIdCommand request, CancellationToken cancellationToken)
        {

            if (request.Order == null)
                throw new CollectException(CollectErrors.OrderNotFound, "Ордер не найден!");

            return Task.FromResult(request.Order.Items.Where(x => x.EcomItemId == request.EComItemId).ToList());
        }
    }
}
