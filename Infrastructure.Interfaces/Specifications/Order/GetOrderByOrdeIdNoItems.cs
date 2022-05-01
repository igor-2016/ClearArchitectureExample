using DataAccess.Interfaces.Dto;
using DataAccess.Interfaces.Specifications.Base;
using System.Linq.Expressions;

namespace DataAccess.Interfaces.Specifications
{
    public class GetOrderByOrdeIdNoItems : Specification<TraceableOrderDto>
    {
        private readonly Guid _orderId;
        public GetOrderByOrdeIdNoItems(Guid orderId)
        {
            _orderId = orderId;
        }

        public override Expression<Func<TraceableOrderDto, bool>> ToExpression()
        {
            return order => order.Id == _orderId;
        }
    }
}
