using AutoMapper;
using DataAccess.Interfaces;
using DataAccess.Interfaces.Dto;
using DataAccess.Interfaces.Specifications.Base;
using Entities.Models.Expansion;
using Microsoft.EntityFrameworkCore;
using Utils;

namespace DataAccess.MsSql
{
    public class AppDbContext : ReadOnlyAppDbContext, IDataAccess
    {
        public AppDbContext(DbContextOptions<AppDbContext> options, IMapper mapper) : base(options, mapper)
        {
            ChangeTracker.AutoDetectChangesEnabled = true;
        }

        public async Task<TEntity> AddNewAsync<TEntity, TEntityDto, TId>(TEntity newEntity, CancellationToken cancellationToken = default)
            where TEntityDto : EntityDto
            where TEntity : Entity<TId>
        {
            var dto = _mapper.Map<TEntityDto>(newEntity);
            await Set<TEntityDto>().AddAsync(dto, cancellationToken);
            await SaveChangesAsync(cancellationToken);
            return _mapper.Map<TEntity>(dto);
        }

        public async Task<TEntity> UpdateAsync<TEntity, TEntityDto, TId>(TEntity updatedEntity,
            Specification<TEntityDto> specification,
            CancellationToken cancellationToken = default)
            where TEntityDto : EntityDto
            where TEntity : Entity<TId>
        {
            var dtoDb = await FindById<TEntityDto, TId>(specification, false, cancellationToken);
               
            // no includes, don't use it  
            //await FindByIdNoIncludesAsync<TEntityDto, TId>(updatedEntity.Id, cancellationToken);

            var updatedDto = _mapper.Map<TEntityDto>(updatedEntity);

            #region check version

            if ((dtoDb is RowVersionEntityDto rowVersionDtoDb && updatedDto is RowVersionEntityDto updatedRowVersionDtoDb)
                &&
               (rowVersionDtoDb.RowVersion.ToVerString() != updatedRowVersionDtoDb.RowVersion.ToVerString()))
            {
                throw new DataAccessException(DbErrors.InvalidEntityVersion,
                    new ExceptionFormatDataEntityVersionInfo<TId>() 
                    { 
                        EntityType = typeof(TEntityDto), 
                        EntityKey = updatedEntity.Id,
                        CurrentDbVersion = rowVersionDtoDb.RowVersion.ToVerString(),
                        TryUpdateToVersion= updatedRowVersionDtoDb.RowVersion.ToVerString()
                    });
            }

            #endregion check version

            _mapper.Map(updatedDto, dtoDb);
            Set<TEntityDto>().Update(dtoDb);
            await SaveChangesAsync(cancellationToken);
            // reloald from db!
            //var updatedDtoDb = await FindById<TEntityDto, TId>(specification, cancellationToken);
            return _mapper.Map<TEntity>(dtoDb);
        }

        public async Task DeleteAsync<TEntityDto, TId>(TId id, CancellationToken cancellationToken = default)
            where TEntityDto : EntityDto
        {
            var dtoDb = await FindByIdNoIncludesAsync<TEntityDto, TId>(id, cancellationToken);
            Set<TEntityDto>().Remove(dtoDb);
            await SaveChangesAsync(cancellationToken);
        }
    }
}
