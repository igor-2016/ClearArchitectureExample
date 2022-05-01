using Entities.Models;
using Entities.Models.Expansion;

namespace Collecting.Interfaces.Requests
{
    public interface ICollectingAnyRequest : IQuery
    {
        TraceableOrder CurrentOrder { get; set; }
        TraceableOrder ChangedOrder { get; set; }
    }
}
