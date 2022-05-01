using ApplicationServices.Implementation.EventHandlers;
using ApplicationServices.Interfaces.ChangeTracker;
using Catalog.Interfaces;
using DomainServices.Interfaces;
using Entities.Models.Expansion;
using Microsoft.Extensions.Logging;
using Utils;
using Workflow.Interfaces.Exceptions;

namespace ApplicationServices.Implementation.ChangeTracker.OrderItems.Handlers
{
    public class AddNewOrderItemHandler : ChangeEntityHandler<TraceableOrderItem, TraceableOrderItem>
    {
        private readonly ILogger _logger;
        private readonly TraceableOrder _orderToBeUpdated;

        private readonly ICatalogService _catalogService;
        private readonly ITransformService _transformService;
        private readonly bool _allowZeroQty;

        public AddNewOrderItemHandler(
            string name,
            TraceableOrder orderToBeUpdated,
            ICatalogService catalogService,
            ITransformService transformService,
            bool allowZeroQty,
            ILogger logger) : base(name)
        {
            _logger = logger;
            _orderToBeUpdated = orderToBeUpdated;
            _catalogService = catalogService;
            _transformService = transformService;
            _allowZeroQty = allowZeroQty;
        }

        public override async Task OnNextChange(ChangeInfo<TraceableOrderItem, TraceableOrderItem> change, 
            CancellationToken cancellationToken)
        {
            if (change == null)
                throw new WorkflowException(WorkflowErrors.NotFoundChanges);

            if (change.ChangeType != ChangeType.Add)
                return;

            if (change?.Source == null)
                throw new WorkflowException(WorkflowErrors.NotFoundSourceOfChangesItem);

            var sourceOfChangeItem = change.Source;

            var items = await new LookupOrderItemByLagerIdHandler(_logger, _catalogService, _transformService)
                .Handle(new LookupOrderItem()
                {
                    Order = _orderToBeUpdated,
                    Data = new LookupItemRequest
                    {
                        LagerId = sourceOfChangeItem.LagerId
                    }
                }, cancellationToken);


            if (items.Count == 0)
                throw new WorkflowException(WorkflowErrors.SKUNotFound, "Товар не найден");

            if (items.Count > 1)
                throw new WorkflowException(WorkflowErrors.MultipleSKUFound, "найдено более 1 позиции товара");

            var item = items.First();


            if (sourceOfChangeItem.OrderQuantity <= 0 || sourceOfChangeItem.OrderQuantity > 100000)
            {
                if (!(_allowZeroQty && (sourceOfChangeItem.OrderQuantity == 0)))
                {
                    throw new WorkflowException(WorkflowErrors.WrongQty, $"неверное количество {sourceOfChangeItem.OrderQuantity} ({sourceOfChangeItem.LagerId})");
                }
            }

           
            await new CheckItemQtyHandler().Handle(
                        new CheckItemQty()
                        {
                            OrderLine = item,
                            Data = new QtyCheckData(sourceOfChangeItem.OrderQuantity)
                        });


            item.OrderQuantity = sourceOfChangeItem.OrderQuantity;
            //item.PickerQuantity = sourceOfChangeItem.PickerQuantity;
            //item.SumOut = item.PriceOut * item.PickerQty.Value;
            //item.SumRozn = item.PriceRozn * item.PickerQty.Value;

            // replacements
            item.ReplacementOnLagerId = sourceOfChangeItem.ReplacementOnLagerId;
            item.ReplacementLagers = sourceOfChangeItem.ReplacementLagers;

            _orderToBeUpdated.Items.Add(item);


            await base.OnNextChange(change, cancellationToken);
        }

        public override Task OnError(Exception ex, CancellationToken cancellationToken)
        {
            throw ex;
        }
    }
}
