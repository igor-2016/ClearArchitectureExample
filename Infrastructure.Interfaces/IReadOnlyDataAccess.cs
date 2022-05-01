using DataAccess.Interfaces.Dto;
using DataAccess.Interfaces.Specifications.Base;
using Entities.Models.Expansion;

namespace DataAccess.Interfaces
{
    public interface IReadOnlyDataAccess
    {
        /// <summary>
        /// Нет вложенных сущностей!
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TEntityDto"></typeparam>
        /// <typeparam name="TId"></typeparam>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<TEntity> GetByIdWithoutIncludes<TEntity, TEntityDto, TId>(TId id, CancellationToken cancellationToken = default)
            where TEntityDto : EntityDto
            where TEntity : Entity<TId>;


        Task<TEntity> GetById<TEntity, TEntityDto, TId>(Specification<TEntityDto> specification,
            CancellationToken cancellationToken = default)
           where TEntityDto : EntityDto
           where TEntity : Entity<TId>;


       Task<IReadOnlyList<TEntity>> FindAsync<TEntity, TEntityDto, TId>(Specification<TEntityDto> specification, CancellationToken cancellationToken = default)
           where TEntityDto : EntityDto
            where TEntity : Entity<TId>;


        Task<TEntity> FindFirstOrDefaultAsync<TEntity, TEntityDto, TId>(Specification<TEntityDto> specification, CancellationToken cancellationToken = default)
            where TEntityDto : EntityDto
             where TEntity : Entity<TId>;
    }
}
