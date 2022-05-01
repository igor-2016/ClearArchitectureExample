using DataAccess.Interfaces.Dto;
using DataAccess.Interfaces.Specifications.Base;
using System.Linq.Expressions;

namespace DataAccess.Interfaces.Specifications
{
    public class GetOrderByBasketIdNoItems : Specification<TraceableOrderDto>
    {
        private readonly Guid _basketId;

        public GetOrderByBasketIdNoItems(Guid basketId)
        {
            _basketId = basketId;
        }

        public override Expression<Func<TraceableOrderDto, bool>> ToExpression()
        {
            return order => order.BasketId == _basketId;
        }
    }
}
