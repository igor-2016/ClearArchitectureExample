using ApplicationServices.Interfaces;
using Collecting.UseCases.Requests;
using Entities.Models.Expansion;
using MediatR;
using Microsoft.Extensions.Logging;
using Utils.Attributes;

namespace Collecting.UseCases
{
    [ApmTrace]
    public class DoneCollectableOrderCommand : IRequest<TraceableOrder>
    {
        public TraceableOrder OrderToBeUpdated { get; set; } // view

        public TraceableOrder SourceOfChanges { get; set; } // TSD input
    }

    public class DoneCollectableOrderCommandHandler :
        CollectingCommandHandlerBase,
        IRequestHandler<DoneCollectableOrderCommand, TraceableOrder>
    {
        ICommonAppService _app;

        public DoneCollectableOrderCommandHandler(
            ILogger<DoneCollectableOrderCommandHandler> logger,
            IMediator mediator,
            ICommonAppService app
            ) : base(mediator, logger)
        {
            _app = app;
        }

        public async Task<TraceableOrder> Handle(DoneCollectableOrderCommand request,
            CancellationToken cancellationToken)
        {
            var  orderToBeUpdated = await _app.AcceptCollecting(
                    new CollectableOrdersInput()
                    {
                        OrderToBeUpdated = request.OrderToBeUpdated,
                        SourceOfChanges = request.SourceOfChanges
                    }, cancellationToken);
            
            return orderToBeUpdated;
        }
    }
}
