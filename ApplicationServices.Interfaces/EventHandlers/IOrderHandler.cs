using Entities.Models.Expansion;

namespace ApplicationServices.Interfaces.EventHandlers
{
    public interface IOrderProcessHandler<TEventData>
    {
        Task<TraceableOrder> Handle(TEventData request, CancellationToken cancellationToken);
    }
}
