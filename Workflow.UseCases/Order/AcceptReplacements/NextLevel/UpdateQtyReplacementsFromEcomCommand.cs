using ApplicationServices.Interfaces;
using Entities.Models.Expansion;
using MediatR;
using Microsoft.Extensions.Logging;
using Utils.Attributes;
using Workflow.UseCases.Order.Dto;

namespace Workflow.UseCases.Order
{
    [ApmTrace]
    public class UpdateQtyReplacementsFromEcomCommand : IRequest<TraceableOrder>
    {
        public TraceableOrder OrderToBeUpdated { get; set; } // view

        public TraceableOrder SourceOfChanges { get; set; } // order data

    }

    public class UpdateQtyReplacementsFromEcomCommandHandler : WorkflowCommandHandlerBase,
        IRequestHandler<UpdateQtyReplacementsFromEcomCommand, TraceableOrder>
    {
        private readonly ICommonAppService _app;
        public UpdateQtyReplacementsFromEcomCommandHandler(
            ILogger<UpdateQtyReplacementsFromEcomCommandHandler> logger,
            IMediator mediator,
            ICommonAppService app
            ) : base(mediator, logger)
        {
            _app = app;
        }

        public const bool AllowZeroQty = true;

        public async Task<TraceableOrder> Handle(UpdateQtyReplacementsFromEcomCommand request, CancellationToken cancellationToken)
        {
            var orderToBeUpdated = await _app.AcceptReplacements(
               new WorkflowCollectableOrdersInput()
               {
                   OrderToBeUpdated = request.OrderToBeUpdated,
                   SourceOfChanges = request.SourceOfChanges
               }, cancellationToken);

            return orderToBeUpdated;
        }
    }
}
