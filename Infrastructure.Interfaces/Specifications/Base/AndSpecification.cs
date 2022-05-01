using DataAccess.Interfaces.Dto;
using System.Linq.Expressions;

namespace DataAccess.Interfaces.Specifications.Base
{
    public class AndSpecification<TDto> : Specification<TDto> where TDto : EntityDto
    {
        private readonly Specification<TDto> _left;
        private readonly Specification<TDto> _right;


        public AndSpecification(Specification<TDto> left, Specification<TDto> right)
        {
            _right = right;
            _left = left;
        }

        public override Expression<Func<TDto, bool>> ToExpression()
        {
            Expression<Func<TDto, bool>> leftExpression = _left.ToExpression();
            Expression<Func<TDto, bool>> rightExpression = _right.ToExpression();

            var paramExpr = Expression.Parameter(typeof(TDto));
            var exprBody = Expression.AndAlso(leftExpression.Body, rightExpression.Body);
            exprBody = (BinaryExpression)new ParameterReplacer(paramExpr).Visit(exprBody);
            var finalExpr = Expression.Lambda<Func<TDto, bool>>(exprBody, paramExpr);

            return finalExpr;
        }
    }
}
