using ApplicationServices.Interfaces.ChangeTracker;
using Catalog.Interfaces;
using DomainServices.Interfaces;
using Entities.Models.Expansion;
using Microsoft.Extensions.Logging;

namespace ApplicationServices.Implementation.EventHandlers
{



    public class ChangeReplacementsHandler : ChangePropertyHandler<TraceableOrderItem, TraceableOrderItem>
    {
        private readonly ILogger _logger;
        private readonly ITransformService _transformService;
        private readonly TraceableOrder _orderToBeUpdated;
        private readonly TraceableOrder _sourceOfChanges;
        private readonly ICatalogService _catalogService;

        public ChangeReplacementsHandler(string name,
            TraceableOrder orderToBeUpdated,
            TraceableOrder sourceOfChanges,
            ILogger logger,
            ICatalogService catalogService,
            ITransformService transformService): base(name)
        {
            _logger = logger;
            _transformService = transformService;
            _orderToBeUpdated = orderToBeUpdated;
            _sourceOfChanges = sourceOfChanges;
            _catalogService = catalogService;
        }

        public override Task OnError(Exception ex, CancellationToken cancellationToken)
        {
            _logger.LogError(nameof(ChangeReplacementsHandler), ex);
            throw ex;
        }

        public override async Task OnNextChange(ChangePropertyInfo<TraceableOrderItem, TraceableOrderItem> change,
            CancellationToken cancellationToken)
        {
            if (change.PropertyName != nameof(TraceableOrderItem.ReplacementLagers))
                return;

            var sourceOfChangesItem = change.Source;
            var itemToBeUpdated = change.Target;

            await ProcessReplacements(_orderToBeUpdated, _sourceOfChanges, sourceOfChangesItem, cancellationToken);
            itemToBeUpdated.ReplacementLagers = sourceOfChangesItem.ReplacementLagers;
            
        }

        private async Task ProcessReplacements(TraceableOrder orderToBeUpdated, TraceableOrder sourceOfChanges, 
            TraceableOrderItem changedItem, CancellationToken cancellationToken)
        {
            // not filled changed item
            if (!_transformService.ItemIsFilled(orderToBeUpdated, changedItem))
            {
                if (!string.IsNullOrEmpty(changedItem.ReplacementLagers))
                {
                    var lagersAsString = changedItem.ReplacementLagers.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    foreach (var lagerAsString in lagersAsString)
                    {
                        var replacementLagerId = 0;
                        if (!int.TryParse(lagerAsString.Trim(), out replacementLagerId))
                        {
                            _logger.LogError("{msg} {lagerId}", "ProcessReplacements: invalid replacement lager id", lagerAsString);
                            continue;
                        }

                        var replacementItem = sourceOfChanges.Items.FirstOrDefault(x => x.LagerId == replacementLagerId); // TODO check only in..
                        if (replacementItem == null)
                        {

                            await new AddItemForReplacementHandler(_logger, _catalogService, _transformService)
                                .Handle(new AddItemForReplacementEvent()
                                {
                                    Order = orderToBeUpdated,
                                    Data = new AddItemForRepData()
                                    {
                                        LagerId = replacementLagerId,
                                        ReplacementLagerId = changedItem.LagerId,
                                    }
                                }, cancellationToken);

                            //var newReplacementItem = await _transformService.CreateNewFromCurrentMinimal(
                            //    replacementLagerId, changedItem, _dateTimeService, cancellationToken)

                            //newReplacementItem.ReplacementOnLagerId = changedItem.LagerId
                            //sourceOfChanges.Items.Add(newReplacementItem)

                        }
                        else
                        {
                            _logger.LogWarning("{msg} {lagerId} {replacementOnLagerId}", "ProcessReplacements: already added lager id", 
                                replacementLagerId, changedItem.LagerId);
                        }
                    }
                }
            }
        }
    }
}
