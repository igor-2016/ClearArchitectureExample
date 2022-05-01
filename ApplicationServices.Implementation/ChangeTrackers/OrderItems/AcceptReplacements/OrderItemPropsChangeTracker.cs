using Entities.Models.Expansion;

namespace ApplicationServices.Implementation.ChangeTracker
{
    public class OrderItemPropsChangeTracker : GetAllChangedProperiesThenProcessTracker<TraceableOrderItem, TraceableOrderItem>
    {
    }
}
