using DataAccess.Interfaces.Dto;
using DataAccess.Interfaces.Specifications.Base;
using System.Linq.Expressions;

namespace DataAccess.Interfaces.Specifications
{
    public class GetOrderByOrdeIdWithItems : Specification<TraceableOrderDto>
    {
        private readonly Guid _orderId;
        public GetOrderByOrdeIdWithItems(Guid orderId)
        {
            _orderId = orderId;
            AddInclude(x => x.Items);
        }

        public override Expression<Func<TraceableOrderDto, bool>> ToExpression()
        {
            return order => order.Id == _orderId;
        }
    }
}
