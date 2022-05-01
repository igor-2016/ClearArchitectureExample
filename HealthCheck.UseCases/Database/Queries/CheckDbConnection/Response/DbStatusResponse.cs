using ApplicationServices.Interfaces;

namespace HealthCheck.UseCases.Database.Queries.CheckDbConnection
{
    public class DbStatusResponse : CheckDataResult<bool>
    {
        protected DbStatusResponse(bool isSuccess, bool data, string reason, Exception error) : base(isSuccess, data, reason, error)
        {
        }
    }
}
