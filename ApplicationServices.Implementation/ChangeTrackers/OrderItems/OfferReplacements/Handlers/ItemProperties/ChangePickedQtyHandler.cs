using ApplicationServices.Interfaces.ChangeTracker;
using Collecting.Interfaces;
using Entities.Models.Expansion;
using Microsoft.Extensions.Logging;

namespace ApplicationServices.Implementation.EventHandlers
{
    public class ChangePickedQtyHandler : ChangePropertyHandler<TraceableOrderItem, TraceableOrderItem>
    {
        private readonly ILogger _logger;
        private readonly bool ReqAdd = false;
        private readonly TraceableOrder _sourceOfChangesOrder;

        public ChangePickedQtyHandler(string name, ILogger logger, TraceableOrder sourceOfChangesOrder) : base(name)
        {
            _logger = logger;
            _sourceOfChangesOrder = sourceOfChangesOrder;
        }

        public override Task OnError(Exception ex, CancellationToken cancellationToken)
        {
            _logger.LogError(nameof(ChangePickedQtyHandler), ex);
            throw ex;
        }

        public override Task OnNextChange(ChangePropertyInfo<TraceableOrderItem, TraceableOrderItem> change,
            CancellationToken cancellationToken)
        {
            if (change.PropertyName != nameof(TraceableOrderItem.PickerQuantity))
                return Task.CompletedTask;

            var newPickedQuantity = change.GetSourceValue<decimal>();
            
            //var sourceOfChangesItem = change.Source
            var itemToBeUpdated = change.Target;


            if(itemToBeUpdated.PickerQuantity.HasValue)
            {
                itemToBeUpdated.PickerQuantity = ReqAdd ? newPickedQuantity + itemToBeUpdated.PickerQuantity : newPickedQuantity;
            }
            else
            {
                itemToBeUpdated.PickerQuantity = newPickedQuantity;
            }

            if (itemToBeUpdated.PickerQuantity < 0)
                throw new CollectException(CollectErrors.ResultQtyLessZero, "Итоговое кол-во меньше нуля");


            //зберігаємо комплектувальника
            itemToBeUpdated.GlobalUserId = _sourceOfChangesOrder.GlobalUserId;
            itemToBeUpdated.PickerName = _sourceOfChangesOrder.PickerName;
            itemToBeUpdated.UserInn = _sourceOfChangesOrder.UserInn;

            //item.SumOut = item.PriceOut * item.PickerQty.Value;
            //item.SumRozn = item.PriceRozn * item.PickerQty.Value;

            return Task.CompletedTask;
        }
    }
}
