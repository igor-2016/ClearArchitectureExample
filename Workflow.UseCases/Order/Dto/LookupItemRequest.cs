using ApplicationServices.Interfaces.Requests;

namespace Workflow.UseCases.Requests
{
    public class LookupItemRequest : ILookupItemRequest
    {
        public Guid? ItemId { get; set; }
        public string Barcode { get; set; }
        public int? LagerId { get; set; }
    }
}
