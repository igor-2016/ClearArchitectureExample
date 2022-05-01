using Entities.Models;
using Entities.Models.Expansion;

namespace ApplicationServices.Interfaces.Requests
{
    public interface ICollectableOrdersInput
    {
        TraceableOrder SourceOfChanges { get; set; }
        
        TraceableOrder OrderToBeUpdated { get; set; }  // output
    }
}
