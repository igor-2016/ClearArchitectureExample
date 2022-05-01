using Collecting.Interfaces.Requests;
using Entities.Models.Expansion;

namespace Collecting.UseCases
{
    public class DefaultCollectingRequest : IDefaultCollectingRequest
    {
        public TraceableOrder CurrentOrder { get; set; }
        public TraceableOrder ChangedOrder { get; set; }
    }
}
