using DataAccess.Interfaces.Dto;
using DataAccess.Interfaces.Specifications.Base;
using Entities.Models.Expansion;

namespace DataAccess.Interfaces
{
    public interface IDataAccess : IReadOnlyDataAccess
    {
        Task<TEntity> AddNewAsync<TEntity, TEntityDto, TId>(TEntity newEntity, CancellationToken cancellationToken = default)
            where TEntityDto : EntityDto
            where TEntity : Entity<TId>;
        Task<TEntity> UpdateAsync<TEntity, TEntityDto, TId>(TEntity updatedEntity,
            Specification<TEntityDto> specification,
            CancellationToken cancellationToken = default)
            where TEntityDto : EntityDto
            where TEntity : Entity<TId>;
        Task DeleteAsync<TEntityDto, TId>(TId id, CancellationToken cancellationToken = default)
            where TEntityDto : EntityDto;
    }
}
