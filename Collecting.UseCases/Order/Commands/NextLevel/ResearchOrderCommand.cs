using ApplicationServices.Interfaces;
using Collecting.UseCases.Requests;
using Entities.Models.Expansion;
using MediatR;
using Microsoft.Extensions.Logging;
using Utils.Attributes;

namespace Collecting.UseCases
{
    [ApmTrace]
    public class ResearchOrderCommand : IRequest<TraceableOrder>
    {
        public TraceableOrder OrderToBeUpdated { get; set; } // view

        public TraceableOrder SourceOfChanges { get; set; } // TSD input
    }

    public class ResearchOrderCommandHandler :
        CollectingCommandHandlerBase,
        IRequestHandler<ResearchOrderCommand, TraceableOrder>
    {
        ICommonAppService _app;

        public ResearchOrderCommandHandler(
            ILogger<DoneCollectableOrderCommandHandler> logger,
            IMediator mediator,
            ICommonAppService app
            ) : base(mediator, logger)
        {
            _app = app;
        }

        public async Task<TraceableOrder> Handle(ResearchOrderCommand request,
            CancellationToken cancellationToken)
        {
            var  orderToBeUpdated = await _app.ResearchOrder(
                    new CollectableOrdersInput()
                    {
                        OrderToBeUpdated = request.OrderToBeUpdated,
                        SourceOfChanges = request.SourceOfChanges
                    }, cancellationToken);
            
            return orderToBeUpdated;
        }
    }
}
