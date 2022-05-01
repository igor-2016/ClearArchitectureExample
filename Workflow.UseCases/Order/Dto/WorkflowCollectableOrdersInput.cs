using ApplicationServices.Interfaces.Requests;
using Entities.Models.Expansion;

namespace Workflow.UseCases.Order.Dto
{
    public class WorkflowCollectableOrdersInput : ICollectableOrdersInput
    {
        public TraceableOrder SourceOfChanges { get; set; }
        public TraceableOrder OrderToBeUpdated { get; set; } // output
    }
}
