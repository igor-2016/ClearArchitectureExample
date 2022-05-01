using ApplicationServices.Interfaces;
using ApplicationServices.Interfaces.ChangeTracker;
using ApplicationServices.Interfaces.Requests;
using Collecting.Interfaces;
using Entities.Models.Expansion;
using Microsoft.Extensions.Logging;
using Workflow.Interfaces.Exceptions;

namespace ApplicationServices.Implementation.EventHandlers
{
    /// <summary>
    /// it sets qty, not picked qty!
    /// </summary>
    public class ChangeOrderQuantityHandler : ChangePropertyHandler<TraceableOrderItem, TraceableOrderItem>
    {
        private readonly ILogger _logger;
        private readonly bool reqAdd = false;


        public ChangeOrderQuantityHandler(string name, ILogger logger) : base(name)
        {
            _logger = logger;
        }

        public override Task OnError(Exception ex, CancellationToken cancellationToken)
        {
            _logger.LogError(nameof(ChangeOrderQuantityHandler), ex);
            throw ex;
        }

        public override async Task OnNextChange(ChangePropertyInfo<TraceableOrderItem, TraceableOrderItem> change,
            CancellationToken cancellationToken)
        {
            if (change.PropertyName != nameof(TraceableOrderItem.OrderQuantity))
                return;

            var newQuantity = change.GetSourceValue<decimal>();
            //var oldQuantity = change.GetTargetValue<decimal>();

            var sourceOfChangesItem = change.Source;
            var itemToBeUpdated = change.Target;


            await new CheckItemQtyHandler().Handle(
                          new CheckItemQty()
                          {
                              OrderLine = sourceOfChangesItem,
                              Data = new QtyCheckData(newQuantity)
                          }, cancellationToken);

           
            itemToBeUpdated.OrderQuantity = reqAdd ? 
                sourceOfChangesItem.OrderQuantity + itemToBeUpdated.OrderQuantity : sourceOfChangesItem.OrderQuantity;


            if (itemToBeUpdated.OrderQuantity < 0)
                throw new WorkflowException(WorkflowErrors.ResultQtyLessZero, "Итоговое кол-во меньше нуля");

            var pickedQty = (itemToBeUpdated.PickerQuantity ?? 0);
            if (pickedQty > 0 && itemToBeUpdated.OrderQuantity < pickedQty)
            {
                // После согласования теперь можем понизить количество ниже собранного
                itemToBeUpdated.PickerQuantity = itemToBeUpdated.OrderQuantity;
                // throw new WorkflowException(WorkflowErrors.WrongQty, "Невозможно указать количество меньше собранного");
            }

        }


        
    }
}
