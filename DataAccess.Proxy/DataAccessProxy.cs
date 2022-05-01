using DataAccess.Interfaces;
using DataAccess.Interfaces.Dto;
using DataAccess.Interfaces.Specifications.Base;
using Entities;
using Entities.Models.Expansion;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Proxy
{
    public class DataAccessProxy : DbContext, IDataAccessProxy
    {
        private readonly IDataAccess _dataAccess;
        
        public DataAccessProxy(IDataAccess dataAccess)
        {
            if(_dataAccess == null)
                throw new ArgumentNullException(nameof(dataAccess));

            _dataAccess = dataAccess;
        }

        public async Task<TEntity> AddNewAsync<TEntity, TEntityDto, TId>(TEntity newEntity, CancellationToken cancellationToken = default)
            where TEntityDto : EntityDto
            where TEntity : Entity<TId>
        {
            if (newEntity == null)
                throw new DataAccessException(DbErrors.EntityIsNull,
                    new ExceptionFormatDataEntityType() { EntityType = typeof(TEntity) });

            try
            {
              return await _dataAccess.AddNewAsync<TEntity, TEntityDto, TId>(newEntity, cancellationToken);
            }
            catch (DataAccessException ex) when (ex.ErrorCode == (int)DbErrors.EntityNotFound)
            {
                throw;
            }
            catch (DataAccessException ex) when (ex.ErrorCode == (int)DbErrors.UnhandledError)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DataAccessException(DbErrors.UnhandledErrorWithEntityType, ex,
                    new ExceptionFormatDataEntityType() 
                    { 
                        EntityType = typeof(TEntityDto) 
                    });
            }
        }

        public async Task DeleteAsync<TEntityDto, TId>(TId id, CancellationToken cancellationToken = default)
            where TEntityDto : EntityDto
        {
            try
            {
                await _dataAccess.DeleteAsync<TEntityDto, TId>(id, cancellationToken);
            }
            catch (DataAccessException ex) when (ex.ErrorCode == (int)DbErrors.EntityNotFound)
            {
                throw;
            }
            catch (DataAccessException ex) when (ex.ErrorCode == (int)DbErrors.GeneralError)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DataAccessException(DbErrors.UnhandledErrorWithEntityKey, ex,
                    new ExceptionFormatDataEntityKey<TId>() 
                    { 
                        EntityType = typeof(TEntityDto), 
                        EntityKey = id 
                    });
            }
        }

        public async Task<IReadOnlyList<TEntity>> FindAsync<TEntity, TEntityDto, TId>(Specification<TEntityDto> specification, 
            CancellationToken cancellationToken = default) 
            where TEntityDto : EntityDto
            where TEntity : Entity<TId>
        {
            try
            {
                return await _dataAccess.FindAsync<TEntity, TEntityDto, TId>(specification, cancellationToken);
            }
            catch (DataAccessException ex) when (ex.ErrorCode == (int)DbErrors.EntityNotFound)
            {
                throw;
            }
            catch (DataAccessException ex) when (ex.ErrorCode == (int)DbErrors.GeneralError)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DataAccessException(DbErrors.UnhandledErrorWithEntityTypeAndSpecification, ex,
                    new ExceptionFormatDataEntityTypeWithSpecification()
                    {
                        EntityType = typeof(TEntityDto),
                        SpecificationType = specification?.GetType()
                    });
            }
        }

        public async Task<TEntity> FindFirstOrDefaultAsync<TEntity, TEntityDto, TId>(Specification<TEntityDto> specification,
            CancellationToken cancellationToken = default) 
            where TEntityDto : EntityDto
            where TEntity : Entity<TId>
        {
            try
            {
                return await _dataAccess.FindFirstOrDefaultAsync<TEntity, TEntityDto, TId>(specification, cancellationToken);
            }
            catch (DataAccessException ex) when (ex.ErrorCode == (int)DbErrors.EntityNotFound)
            {
                throw;
            }
            catch (DataAccessException ex) when (ex.ErrorCode == (int)DbErrors.GeneralError)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DataAccessException(DbErrors.UnhandledErrorWithEntityTypeAndSpecification, ex,
                    new ExceptionFormatDataEntityTypeWithSpecification()
                    {
                        EntityType = typeof(TEntityDto),
                        SpecificationType = specification?.GetType()
                    });
            }
        }

        public virtual async Task<TEntity> GetById<TEntity, TEntityDto, TId>(Specification<TEntityDto> specification,
            CancellationToken cancellationToken = default)
           where TEntityDto : EntityDto
           where TEntity : Entity<TId>
        {
            try
            {
                return await _dataAccess.GetById<TEntity, TEntityDto, TId>(specification, cancellationToken);
            }
            catch (DataAccessException ex) when (ex.ErrorCode == (int)DbErrors.EntityNotFound)
            {
                throw;
            }
            catch (DataAccessException ex) when (ex.ErrorCode == (int)DbErrors.GeneralError)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DataAccessException(DbErrors.UnhandledErrorWithEntityTypeAndSpecification, ex,
                    new ExceptionFormatDataEntityTypeWithSpecification()
                    {
                        SpecificationType = typeof(TEntityDto),
                        EntityType = typeof(TEntityDto),
                    });
            }
        }

        public virtual async Task<TEntity> GetByIdWithoutIncludes<TEntity, TEntityDto, TId>(TId id, CancellationToken cancellationToken = default)
            where TEntityDto : EntityDto
            where TEntity : Entity<TId>
        {
            try
            {
                return  await _dataAccess.GetByIdWithoutIncludes<TEntity, TEntityDto, TId>(id, cancellationToken);   
            }
            catch (DataAccessException ex) when (ex.ErrorCode == (int)DbErrors.EntityNotFound)
            {
                throw;
            }
            catch (DataAccessException ex) when (ex.ErrorCode == (int)DbErrors.GeneralError)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DataAccessException(DbErrors.UnhandledErrorWithEntityKey, ex,
                    new ExceptionFormatDataEntityKey<TId>() 
                    { 
                        EntityType = typeof(TEntityDto), 
                        EntityKey = id 
                    });
            }
        }

        public async Task<TEntity> UpdateAsync<TEntity, TEntityDto, TId>(TEntity updatedEntity,
            Specification<TEntityDto> specification,
            CancellationToken cancellationToken = default)
           where TEntityDto : EntityDto
           where TEntity : Entity<TId>
        {
            if (updatedEntity == null)
                throw new DataAccessException(DbErrors.EntityIsNull, 
                    new ExceptionFormatDataEntityType() { EntityType = typeof(TEntity)});

            try
            {
                return await _dataAccess.UpdateAsync<TEntity, TEntityDto, TId>(updatedEntity, specification, cancellationToken);
            }
            catch (DataAccessException ex) when (ex.ErrorCode == (int)DbErrors.EntityNotFound)
            {
                throw;
            }
            catch (DataAccessException ex) when (ex.ErrorCode == (int)DbErrors.InvalidEntityVersion)
            {
                throw;
            }
            catch (DataAccessException ex) when (ex.ErrorCode == (int)DbErrors.GeneralError)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DataAccessException(DbErrors.UnhandledErrorWithEntityKey, ex, 
                    new ExceptionFormatDataEntityKey<TId>() 
                    { 
                        EntityType = typeof(TEntityDto), 
                        EntityKey = updatedEntity.Id 
                    });
            }
        }
    }
}