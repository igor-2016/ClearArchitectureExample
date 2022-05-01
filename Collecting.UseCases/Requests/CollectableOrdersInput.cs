using ApplicationServices.Interfaces;
using ApplicationServices.Interfaces.Requests;
using Entities.Models;
using Entities.Models.Expansion;

namespace Collecting.UseCases.Requests
{
    public class CollectableOrdersInput : ICollectableOrdersInput
    {
        public TraceableOrder SourceOfChanges { get; set; }
        public TraceableOrder OrderToBeUpdated { get; set; } // output
    }
}
