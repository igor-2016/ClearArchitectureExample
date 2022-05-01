using ECom.Entities.Models.Workflow;
using Workflow.Interfaces.Clients.Responses;

namespace Workflow.Interfaces.Clients
{
    public interface IWorkflowServiceClient
    {
        Task<EmptyDataResult> OnChangeState<TData>(Guid basketId, int currentStateId, int nextStateId, TData data, CancellationToken cancellationToken);

        Task<EmptyDataResult> OnChangeStateNoData(Guid basketId, int currentStateId, int nextStateId, CancellationToken cancellationToken);

        Task<WorkflowOrderState> GetOrderCurrentState(Guid basketId, CancellationToken cancellationToken);
    }
}
