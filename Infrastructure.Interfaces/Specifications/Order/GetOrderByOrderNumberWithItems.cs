using DataAccess.Interfaces.Dto;
using DataAccess.Interfaces.Specifications.Base;
using System.Linq.Expressions;

namespace DataAccess.Interfaces.Specifications
{
    public class GetOrderByOrderNumberWithItems : Specification<TraceableOrderDto>
    {
        private readonly string _orderNumber;
        public GetOrderByOrderNumberWithItems(string orderNumber)
        {
            _orderNumber = orderNumber;
            AddInclude(x => x.Items);
        }

        public override Expression<Func<TraceableOrderDto, bool>> ToExpression()
        {
            return order => order.OrderNumber == _orderNumber;
        }
    }
}
