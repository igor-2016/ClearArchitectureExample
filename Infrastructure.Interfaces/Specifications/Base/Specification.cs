using DataAccess.Interfaces.Dto;
using System.Linq.Expressions;

namespace DataAccess.Interfaces.Specifications.Base
{
    public abstract class Specification<TDto> where TDto: EntityDto
    {
        public abstract Expression<Func<TDto, bool>> ToExpression();

        public List<Expression<Func<TDto, object>>> Includes { get; } = new List<Expression<Func<TDto, object>>>();

        //TODO order by asc desc, take from, 

        protected virtual Specification<TDto> AddInclude(Expression<Func<TDto, object>> includeExpression)
        {
            Includes.Add(includeExpression);
            return this;
        }

        public bool IsSatisfiedBy(TDto entity)
        {
            Func<TDto, bool> predicate = ToExpression().Compile();
            return predicate(entity);
        }


        public Specification<TDto> And(Specification<TDto> specification)
        {
            return new AndSpecification<TDto>(this, specification);
        }


        public Specification<TDto> Or(Specification<TDto> specification)
        {
            return new OrSpecification<TDto>(this, specification);
        }
    }
}
