using ApplicationServices.Interfaces.Requests;

namespace ApplicationServices.Implementation.EventHandlers
{
    public class LookupItemRequest : ILookupItemRequest
    {
        public Guid? ItemId { get; set; }
        public string Barcode { get; set; }
        public int? LagerId { get; set; }
    }
}
