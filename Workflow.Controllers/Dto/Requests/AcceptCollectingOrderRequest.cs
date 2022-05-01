
using ECom.Entities.Models;

namespace Workflow.Controllers.Dto.Requests
{
    public class AcceptCollectingOrderRequest
    {
        public List<AcceptedCollectedItem> Items { get; set; }
    }
}
