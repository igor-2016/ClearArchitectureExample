using Entities.Models.Expansion;

namespace ApplicationServices.Implementation.ChangeTracker
{
    public class OrderItemsChangeTracker : GetAllChangesThenProcessEntityTracker<TraceableOrderItem, TraceableOrderItem>
    {
        public override Task<TraceableOrderItem?> SearchInSourcesByCurrentTarget
            (IEnumerable<TraceableOrderItem> sources, TraceableOrderItem target, CancellationToken cancellationToken)
        {
            return Task.FromResult(sources.FirstOrDefault(x => x.LagerId == target.LagerId));
        }
    }
}
