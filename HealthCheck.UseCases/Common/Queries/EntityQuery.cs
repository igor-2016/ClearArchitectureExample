using AutoMapper;
using DataAccess.Interfaces;
using Entities;
using MediatR;

namespace HealthCheck.UseCases.Common.Queries
{
    public abstract class EntityQuery<TDto> : IRequest<TDto>
    {
        public int Id { get; set; }
    }

    public abstract class EntityQueryHandler<TRequest, TEntity, TDto> : IRequestHandler<TRequest, TDto>
        where TEntity : Entity
        where TRequest : EntityQuery<TDto>
    {
        protected readonly IReadOnlyDataAccess DataAccess;
        protected readonly IMapper EntityMapper;

        protected EntityQueryHandler(IReadOnlyDataAccess dataAccess, IMapper mapper)
        {
            DataAccess = dataAccess;
            EntityMapper = mapper;
        }


        public abstract Task<TDto> Handle(TRequest request, CancellationToken cancellationToken);
       
    }

}
