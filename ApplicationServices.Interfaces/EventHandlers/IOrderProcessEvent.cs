using Entities.Models.Expansion;

namespace ApplicationServices.Interfaces
{
    public interface IOrderProcessEvent<TEventData>
    {
        TraceableOrder Order { get; set; }

        TEventData Data { get; set; }
    }
}
