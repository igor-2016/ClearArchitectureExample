using Entities.Models.Expansion;

namespace ApplicationServices.Interfaces
{
    public interface IOrderItemProcessEvent<TEventData> 
    {
        TraceableOrderItem OrderLine { get; set; }

        TEventData Data { get; set; }
    }
}
