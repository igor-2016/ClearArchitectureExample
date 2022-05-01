using DataAccess.Interfaces.Dto;
using DataAccess.Interfaces.Specifications.Base;
using System.Linq.Expressions;

namespace DataAccess.Interfaces.Specifications
{
    public class GetOrderByBasketIdWithItems : Specification<TraceableOrderDto>
    {
        private readonly Guid _basketId;

        public GetOrderByBasketIdWithItems(Guid basketId)
        {
            _basketId = basketId;

            AddInclude(x => x.Items);
        }

        public override Expression<Func<TraceableOrderDto, bool>> ToExpression()
        {
            return order => order.BasketId == _basketId;
        }
    }
}
