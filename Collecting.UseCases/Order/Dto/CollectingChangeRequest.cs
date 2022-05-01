using Collecting.Interfaces.Requests;
using Entities.Models.Collecting;
using Entities.Models.Expansion;

namespace Collecting.UseCases
{
    public class CollectingChangeRequest : ICollectingChangeRequest
    {
        public TraceableOrder CurrentOrder { get; set; }
        //public TraceableOrder ChangedOrder { get; set; }
        public OrderData ChangedOrder { get; set; }
    }
}
