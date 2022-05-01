using Collecting.Interfaces;
using Entities.Models;
using Entities.Models.Expansion;
using MediatR;

namespace Collecting.UseCases
{
    public class CheckItemQtyCommand : IRequest
    {
        public TraceableOrderItem OrderLine { get; set; }
        public decimal Qty { get; set; }
    }

    public class CheckItemQtyCommandHandler : AsyncRequestHandler<CheckItemQtyCommand>
    {
   

        public CheckItemQtyCommandHandler()
        {
       
        }

        protected override async Task Handle(CheckItemQtyCommand request, CancellationToken cancellationToken)
        {
            if(!request.OrderLine.IsWeighted.HasValue)
            {
                throw new CollectException(CollectErrors.IsWeightedNotSet, "Не указано признак вессового товара!");
            }

            //для невесовых товаров кол-во должно быть целым числом
            if (!request.OrderLine.IsWeighted.Value)
            {
                if (request.Qty % 1 != 0)
                {
                    throw new CollectException(CollectErrors.WrongQty, "Нельзя иметь дробное количество товара.");
                }
            }

            await Task.CompletedTask;
        }
    }
}
