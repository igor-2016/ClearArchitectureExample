using DataAccess.Interfaces;
using DataAccess.Interfaces.Dto;
using Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ECom.Expansion.UnitTests.DataAccess.MsSql.Mocks
{
    public class DataAccessMock : IDataAccess
    {
        private readonly Action? _onAddNew;
        private readonly Action? _onGet;
        private readonly Action? _onUpdate;
        private readonly Action? _onDelete;

        private readonly IDataAccess _dataAccess;

        public DataAccessMock(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        public DataAccessMock(Action? onAddNew = default, Action? onGet = default,  Action? onUpdate = default, Action? onDelete = default)
        {
            _onAddNew = onAddNew;
            _onGet = onGet;
            _onUpdate = onUpdate;
            _onDelete = onDelete;
        }

        public async Task<TEntity> AddNewAsync<TEntity, TEntityDto>(TEntity newEntity, CancellationToken cancellationToken = default)
            where TEntityDto : EntityDto
            where TEntity : Entity
        {
            if (_dataAccess != null)
                return await _dataAccess.AddNewAsync<TEntity, TEntityDto>(newEntity, cancellationToken);

            if(_onAddNew   != null)
                _onAddNew();

            return newEntity;
        }

        public Task DeleteAsync<TEntityDto>(Guid id, CancellationToken cancellationToken = default)
            where TEntityDto : EntityDto
        {
            if (_dataAccess != null)
            {
                _dataAccess.DeleteAsync<TEntityDto>(id, cancellationToken);
                return Task.CompletedTask;
            }

            if(_onDelete != null)
                _onDelete();

            return Task.CompletedTask;
        }

        public async Task<TEntity> GetById<TEntity, TEntityDto>(Guid id, CancellationToken cancellationToken = default)
            where TEntityDto : EntityDto
            where TEntity : Entity
        {
            if (_dataAccess != null)
                return await _dataAccess.GetById<TEntity, TEntityDto>(id, cancellationToken);


            if (_onGet != null)
                _onGet();

            return default;
        }

        public async Task<TEntity> UpdateAsync<TEntity, TEntityDto>(TEntity updatedEntity, CancellationToken cancellationToken = default)
            where TEntityDto : EntityDto
            where TEntity : Entity
        {
            if (_dataAccess != null)
                return await _dataAccess.UpdateAsync<TEntity, TEntityDto>(updatedEntity, cancellationToken);

            if(_onUpdate != null)
                _onUpdate();

            return updatedEntity;
        }
    }
}
