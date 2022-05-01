using ApplicationServices.Interfaces.ChangeTracker;
using Entities.Models.Expansion;
using Microsoft.Extensions.Logging;
using Utils;
using Workflow.Interfaces.Exceptions;

namespace ApplicationServices.Implementation.ChangeTracker.OrderItems.Handlers
{
    public class UpdateOrderItemHandler : ChangeEntityHandler<TraceableOrderItem, TraceableOrderItem>
    {
        private readonly ILogger _logger;
        //private readonly TraceableOrder _orderToBeUpdated;

        public UpdateOrderItemHandler(
            string name,
            //TraceableOrder orderToBeUpdated,
            ILogger logger,
            IChangeTracker<TraceableOrderItem, TraceableOrderItem,
                ChangePropertyInfo<TraceableOrderItem, TraceableOrderItem>> propertiesTracker) : base(name, propertiesTracker)
        {
            _logger = logger;
            //_orderToBeUpdated = orderToBeUpdated;
        }

        public override async Task OnNextChange(ChangeInfo<TraceableOrderItem, TraceableOrderItem> change, 
            CancellationToken cancellationToken)
        {
            if (change == null)
                throw new WorkflowException(WorkflowErrors.NotFoundChanges);

            if (change.ChangeType != ChangeType.Update)
                return;

            if (change?.Target == null)
                throw new WorkflowException(WorkflowErrors.NotFoundTargetItem);

            if (change?.Source == null)
                throw new WorkflowException(WorkflowErrors.NotFoundSourceOfChangesItem);

            await base.OnNextChange(change, cancellationToken);
        }

        public override Task OnError(Exception ex, CancellationToken cancellationToken)
        {
            throw ex;
        }
    }
}
