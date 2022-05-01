using ApplicationServices.Interfaces;
using Collecting.UseCases.Requests;
using Entities.Models.Expansion;
using MediatR;
using Microsoft.Extensions.Logging;
using Utils.Attributes;

namespace Collecting.UseCases
{
    [ApmTrace]
    public class UpdateCollectableOrderCommand : IRequest<TraceableOrder>
    {
        public TraceableOrder OrderToBeUpdated { get; set; } // view

        public TraceableOrder SourceOfChanges { get; set; } // TSD input
    }

    public class UpdateCollectableOrderCommandHandler :
        CollectingCommandHandlerBase,
        IRequestHandler<UpdateCollectableOrderCommand, TraceableOrder>
    {
        ICommonAppService _app;

        public UpdateCollectableOrderCommandHandler(
            ILogger<UpdateCollectableOrderCommandHandler> logger,
            IMediator mediator,
            ICommonAppService app
            ) : base(mediator, logger)
        {
            _app = app;
        }

        public async Task<TraceableOrder> Handle(UpdateCollectableOrderCommand request,
            CancellationToken cancellationToken)
        {
             var orderToBeUpdated = await _app.OfferReplacements(
                    new CollectableOrdersInput()
                    {
                        OrderToBeUpdated = request.OrderToBeUpdated,
                        SourceOfChanges = request.SourceOfChanges
                    }, cancellationToken);
           
            return orderToBeUpdated;
        }
    }
}
