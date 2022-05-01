using Entities.Models.Expansion;

namespace DomainServices.Interfaces.Delegates
{
    public delegate void IsFilledCalculator(IEnumerable<TraceableOrderItem> items);
}
