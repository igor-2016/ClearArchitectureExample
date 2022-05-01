using DataAccess.Interfaces;
using DataAccess.Interfaces.Dto;
using DataAccess.Interfaces.Specifications.Base;
using Entities;
using Entities.Models.Expansion;
using Microsoft.EntityFrameworkCore;

namespace ReadOnlyDataAccess.Proxy
{
    public class ReadOnlyDataAccessProxy : DbContext, IReadOnlyDataAccessProxy
    {
        private readonly IReadOnlyDataAccess _dataAccess;
        
        public ReadOnlyDataAccessProxy(IReadOnlyDataAccess dataAccess)
        {
            if(_dataAccess == null)
                throw new ArgumentNullException(nameof(dataAccess));

            _dataAccess = dataAccess;
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

        public async Task<TEntity> FindFirstOrDefaultAsync<TEntity, TEntityDto, TId>(
            Specification<TEntityDto> specification, CancellationToken cancellationToken = default) 
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
                        EntityType = typeof(TEntityDto),
                        SpecificationType = specification?.GetType()
                    });
            }
        }

        public virtual async Task<TEntity> GetByIdWithoutIncludes<TEntity, TEntityDto, TId>(TId id, CancellationToken cancellationToken = default)
            where TEntityDto : EntityDto
            where TEntity : Entity<TId>
        {
            try
            {
                return  await _dataAccess.GetByIdWithoutIncludes<TEntity, TEntityDto, TId> (id, cancellationToken);   
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

        
    }
}