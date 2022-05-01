using ApplicationServices.Interfaces;
using Collecting.Interfaces;
using Entities.Models.Expansion;

namespace ApplicationServices.Implementation.EventHandlers
{
    public class QtyCheckData
    {
        public decimal Qty { get; }
        public QtyCheckData(decimal qty)
        {
            Qty = qty;
        }
    }
    public class CheckItemQty : IOrderItemProcessEvent<QtyCheckData>
    {
        public TraceableOrderItem OrderLine { get; set; }
        public QtyCheckData Data { get; set; }
    }

    public class CheckItemQtyHandler 
    {
   
        public async Task Handle(CheckItemQty data, CancellationToken cancellationToken = default)
        {
            var qtyToCheck = data.Data.Qty;
            if(!data.OrderLine.IsWeighted.HasValue)
            {
                throw new CollectException(CollectErrors.IsWeightedNotSet, "Не указано признак вессового товара!");
            }

            //для невесовых товаров кол-во должно быть целым числом
            if (!data.OrderLine.IsWeighted.Value)
            {
                if (qtyToCheck % 1 != 0)
                {
                    throw new CollectException(CollectErrors.WrongQty, "Нельзя иметь дробное количество товара.");
                }
            }

            await Task.CompletedTask;
        }
    }
}
