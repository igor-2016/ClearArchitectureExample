using Entities.Models.Expansion;

namespace ApplicationServices.Implementation.ChangeTracker
{
    public class OrderPropsChangeTracker : GetAllChangedProperiesThenProcessTracker<TraceableOrder, TraceableOrder>
    {
    }
}
