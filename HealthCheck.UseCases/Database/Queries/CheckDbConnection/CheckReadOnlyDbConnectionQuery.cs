using ApplicationServices.Interfaces;
using DataAccess.Interfaces;
using DataAccess.Interfaces.Dto;
using Entities.Models.Expansion;
using MediatR;

namespace HealthCheck.UseCases.Database.Queries.CheckDbConnection
{
    public class CheckReadOnlyDbConnectionQuery : IRequest<DbStatusResponse>
    {
        public DbConnectionInfoRequest Request { get; set; }
    }

    public class CheckReadOnlyDbConnectionQueryHandler : IRequestHandler<CheckReadOnlyDbConnectionQuery, CheckDataResult<bool>>
    {
        private readonly IDataAccess _dataAccess;
        private readonly IReadOnlyDataAccess _readOnlyDataAccess;
        public CheckReadOnlyDbConnectionQueryHandler(IDataAccess dataAccess, IReadOnlyDataAccess readOnlyDataAccess)
        {
            _dataAccess = dataAccess;
            _readOnlyDataAccess = readOnlyDataAccess;
        }
        public async Task<CheckDataResult<bool>> Handle(CheckReadOnlyDbConnectionQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // for starting
                return DbStatusResponse.Success(true);

                var tasks = new[] { PingDatabase(_dataAccess, cancellationToken), PingDatabase(_readOnlyDataAccess, cancellationToken) };
                var allTasks = await Task.WhenAll(tasks);
                return (allTasks.All(x => x) ? DbStatusResponse.Success(true) : DbStatusResponse.Fail(""));
            }
            catch (Exception ex)
            {
                return DbStatusResponse.Error(ex);
            }
        }

        protected async Task<bool> PingDatabase(IReadOnlyDataAccess dataAccess, CancellationToken cancellationToken)
        {
            bool databaseIsAvailable;
            try
            {
                await dataAccess.GetByIdWithoutIncludes<TraceableOrder, TraceableOrderDto, Guid>(Guid.NewGuid(), cancellationToken);
                databaseIsAvailable = true;
            }
            catch (DataAccessException dbEx) when (dbEx.ErrorCode == (int)DbErrors.EntityNotFound)
            {
                databaseIsAvailable = true;
            }
            return databaseIsAvailable;
        }
    }
}