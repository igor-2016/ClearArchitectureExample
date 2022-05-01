using Entities.Models.Expansion;

namespace DomainServices.Interfaces.Delegates
{
    public delegate void RowNumCalculator(IEnumerable<TraceableOrderItem> items);
}
