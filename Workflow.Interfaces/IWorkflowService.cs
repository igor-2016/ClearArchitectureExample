using ECom.Entities.Models.Workflow;
using Entities.Models.Expansion;

namespace Workflow.Interfaces
{
    public interface IWorkflowService
    {
        Task<TraceableOrder> CreateOrder(TraceableOrder newOrder, CancellationToken cancellationToken);
        Task<TraceableOrder> UpdateOrder(TraceableOrder order, CancellationToken cancellationToken);
       
        Task<TraceableOrder> GetOrderByBasketId(Guid basketId, CancellationToken cancellationToken);


        Task StartCollecting(int oldOrderStatusId, TraceableOrder order,
            CancellationToken cancellationToken);


        Task UpdateCollecting(int oldOrderStatusId, TraceableOrder order,
            CancellationToken cancellationToken);


        Task DoneCollecting(int oldOrderStatusId, TraceableOrder order,
            CancellationToken cancellationToken); 
        
        Task DefaultAction(int oldOrderStatusId, TraceableOrder order,
            CancellationToken cancellationToken);

        Task DefaultAction(Guid basketId, int oldOrderStatusId, int newOrderStatusId, CancellationToken cancellationToken);

        Task<WorkflowOrderState> GetOrderCurrentState(Guid basketId, CancellationToken cancellationToken);

    }
}