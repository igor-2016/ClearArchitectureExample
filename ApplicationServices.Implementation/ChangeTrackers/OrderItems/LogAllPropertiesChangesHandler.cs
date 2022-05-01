using ApplicationServices.Interfaces.ChangeTracker;
using Entities.Models.Expansion;
using Microsoft.Extensions.Logging;
using Utils;

namespace ApplicationServices.Implementation.EventHandlers
{
    public class LogAllPropertiesChangesHandler : ChangePropertyHandler<TraceableOrderItem, TraceableOrderItem>
    {
        private readonly ILogger _logger;

        public LogAllPropertiesChangesHandler(string name, ILogger logger) : base(name)
        {
            _logger = logger;
        }

        public override Task OnError(Exception ex, CancellationToken cancellationToken)
        {
            _logger.LogError(nameof(LogAllPropertiesChangesHandler), ex);
            throw ex;
        }

        public override Task OnNextChange(ChangePropertyInfo<TraceableOrderItem, TraceableOrderItem> change,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation(nameof(LogAllPropertiesChangesHandler), change.ToJson());
            return Task.CompletedTask;
        }
    }
}
