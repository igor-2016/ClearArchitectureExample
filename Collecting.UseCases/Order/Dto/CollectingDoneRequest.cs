using Collecting.Interfaces.Requests;
using Entities.Models;
using Entities.Models.Expansion;

namespace Collecting.UseCases
{
    public class CollectingDoneRequest : ICollectingDoneRequest
    {
        public TraceableOrder CurrentOrder { get; set; }
        public TraceableOrder ChangedOrder { get; set; }
    }
}
