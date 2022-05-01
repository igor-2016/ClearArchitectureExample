using AutoMapper;
using AutoMapper.QueryableExtensions;
using DataAccess.Interfaces;
using DataAccess.Interfaces.Dto;
using DataAccess.Interfaces.Specifications.Base;
using Entities.Models.Expansion;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.MsSql
{
    public class ReadOnlyAppDbContext : DbContext, IReadOnlyDataAccess
    {
        protected readonly IMapper _mapper;

        public ReadOnlyAppDbContext(DbContextOptions options, IMapper mapper) : base(options)
        {
            ChangeTracker.AutoDetectChangesEnabled = false;
            _mapper = mapper;
        }

        public virtual async Task<TEntity> GetById<TEntity, TEntityDto, TId>(
            Specification<TEntityDto> specification,
            CancellationToken cancellationToken = default)
            where TEntityDto : EntityDto
            where TEntity : Entity<TId>
        {
            var dto = await FindById<TEntityDto, TId>(specification, true, cancellationToken);
            return _mapper.Map<TEntity>(dto);
        }

        public virtual async Task<TEntity> GetByIdWithoutIncludes<TEntity, TEntityDto, TId>(TId id, 
            CancellationToken cancellationToken = default)
            where TEntityDto : EntityDto 
            where TEntity : Entity<TId>
        {
            var dto = await FindByIdNoIncludesAsync<TEntityDto, TId>(id, cancellationToken);
            return _mapper.Map<TEntity>(dto);
        }




        public virtual async Task<IReadOnlyList<TEntity>> FindAsync<TEntity, TEntityDto, TId>(
            Specification<TEntityDto> specification, 
            CancellationToken cancellationToken = default)
            where TEntityDto : EntityDto
            where TEntity : Entity<TId>
        {
            var query = Set<TEntityDto>().AsNoTracking().AsQueryable();

            query = specification.Includes.Aggregate(query, (current, include) => current.Include(include));

            query = query.Where(specification.ToExpression());

            return await query
                .ProjectTo<TEntity>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);
        }

        public virtual async Task<TEntity> FindFirstOrDefaultAsync<TEntity, TEntityDto, TId>(
            Specification<TEntityDto> specification, 
            CancellationToken cancellationToken = default)
            where TEntityDto : EntityDto
            where TEntity : Entity<TId>
        {
            var query = Set<TEntityDto>().AsNoTracking().AsQueryable();

            query = specification.Includes.Aggregate(query, (current, include) => current.Include(include));

            query = query.Where(specification.ToExpression());

            var dto = await query.FirstOrDefaultAsync(cancellationToken);

            return _mapper.Map<TEntity>(dto);
        }



        /// <summary>
        /// Нет вложений использовать только для загрузки одной сущности!
        /// </summary>
        /// <typeparam name="TEntityDto"></typeparam>
        /// <typeparam name="TId"></typeparam>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="DataAccessException"></exception>
        protected virtual async Task<TEntityDto> FindByIdNoIncludesAsync<TEntityDto, TId>(TId id, 
            CancellationToken cancellationToken = default)
            where TEntityDto : EntityDto
        {
            var dtoDb = await Set<TEntityDto>().FindAsync(new object[] { id }, cancellationToken);

            if (dtoDb == null)
                throw new DataAccessException(DbErrors.EntityNotFound,
                    new ExceptionFormatDataEntityKey<TId>() { EntityType = typeof(TEntityDto), EntityKey = id });

            // TODO includes added
            //await Entry(dtoDb).Collection("").LoadAsync();
            return dtoDb;
        }

        protected virtual async Task<TEntityDto> FindById<TEntityDto, TId>(
            Specification<TEntityDto> specification, bool withNoTracking,
            CancellationToken cancellationToken)
            where TEntityDto : EntityDto
        {
            var query = withNoTracking ? Set<TEntityDto>().AsNoTracking().AsQueryable() : Set<TEntityDto>().AsQueryable();

            query = specification.Includes.Aggregate(query, (current, include) => current.Include(include));

            query = query.Where(specification.ToExpression());

            var dtoDb = await query.FirstOrDefaultAsync(cancellationToken);

            if (dtoDb == null)
                throw new DataAccessException(DbErrors.EntityNotFound,
                    new ExceptionFormatDataEntityTypeWithSpecification() 
                    { 
                        SpecificationType = specification?.GetType(),
                        EntityType = typeof(TEntityDto)
                    });

            return dtoDb;
        }


        #region db mapping 

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasDefaultSchema("Expansion");



            modelBuilder.Entity<TraceableOrderDto>(b => BuildFozzyOrders(b));
            modelBuilder.Entity<TraceableOrderItemDto>(b => BuildFozzyOrderItem(b));

        }
        private void BuildFozzyOrders(EntityTypeBuilder<TraceableOrderDto> b)
        {
            b.ToTable("Orders", "Expansion");
            b.HasKey(x => x.Id);//.IsClustered(false);
            b.HasMany(x => x.Items).WithOne().HasForeignKey(x => x.OrderId);
            b.HasIndex(e => e.BasketId)
                .IsUnique()
                .HasName("UX_FozzyOrder_BasketId");
            //.IsClustered(true);
            b.Property(e => e.RowVersion).IsConcurrencyToken().ValueGeneratedOnAddOrUpdate();
            //b.Property(e => e.RowVersion).ValueGeneratedOnAddOrUpdate();
        }

        private void BuildFozzyOrderItem(EntityTypeBuilder<TraceableOrderItemDto> b)
        {
            b.ToTable("OrderItems", "Expansion");
            b.HasKey(b => b.Id);//.IsClustered(false);
            b.HasIndex(e => e.BasketId).HasName("IX_FozzyOrderItems_BasketId");
            b.Property(b => b.Id).ValueGeneratedNever();
            //b.HasMany(x => x.Items).WithOne().HasForeignKey(x => x.ParentId);
        }

        #endregion db mapping

    }
}
