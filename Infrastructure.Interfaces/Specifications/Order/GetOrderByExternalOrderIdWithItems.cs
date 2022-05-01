using DataAccess.Interfaces.Dto;
using DataAccess.Interfaces.Specifications.Base;
using System.Linq.Expressions;

namespace DataAccess.Interfaces.Specifications
{
    public class GetOrderByExternalOrderIdWithItems : Specification<TraceableOrderDto>
    {
        private readonly int _externalOrderId;
        public GetOrderByExternalOrderIdWithItems(int externalOrderId)
        {
            _externalOrderId = externalOrderId;

            AddInclude(x => x.Items);
        }

        public override Expression<Func<TraceableOrderDto, bool>> ToExpression()
        {
            return order => order.ExternalOrderId == _externalOrderId;
        }
    }
}
