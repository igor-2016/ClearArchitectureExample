using ApplicationServices.Interfaces.ChangeTracker;
using Collecting.Interfaces;
using Entities.Models.Expansion;
using Microsoft.Extensions.Logging;
using Utils;
using Workflow.Interfaces.Exceptions;

namespace ApplicationServices.Implementation.ChangeTracker.OrderItems.Handlers
{
    public class RemoveCurrentOrderItemHandler : ChangeEntityHandler<TraceableOrderItem, TraceableOrderItem>
    {
        private readonly ILogger _logger;
        private readonly TraceableOrder _orderToBeUpdated;

        public RemoveCurrentOrderItemHandler(
            string name,
            TraceableOrder orderToBeUpdated,
            ILogger logger) : base(name)
        {
            _logger = logger;
            _orderToBeUpdated = orderToBeUpdated;
        }

        public override async Task OnNextChange(ChangeInfo<TraceableOrderItem, TraceableOrderItem> change, 
            CancellationToken cancellationToken)
        {
            if (change == null)
                throw new WorkflowException(WorkflowErrors.NotFoundChanges);

            if (change.ChangeType != ChangeType.Remove)
                return;

            if (change?.Target == null)
                throw new WorkflowException(WorkflowErrors.NotFoundTargetItem);

            var toDeleteItem = change.Target;

            var itemFound = _orderToBeUpdated.Items.FirstOrDefault(x => x.Id == toDeleteItem.Id);

            if (itemFound == null)
                throw new WorkflowException(WorkflowErrors.PositionNotFound, "Строка для удаления не найдена");

            _orderToBeUpdated.Items.Remove(itemFound);


            await base.OnNextChange(change, cancellationToken);
        }

        public override Task OnError(Exception ex, CancellationToken cancellationToken)
        {
            throw ex;
        }
    }
}
