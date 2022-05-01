using Entities.Models;
using Entities.Models.Collecting;
using Entities.Models.Expansion;

namespace Collecting.Interfaces.Requests
{

    public interface ICollectingChangeRequest : IQuery
    {
        public TraceableOrder CurrentOrder { get; set; }
        //TraceableOrder ChangedOrder { get; set; }
        public OrderData ChangedOrder { get; set; }
    }
}
