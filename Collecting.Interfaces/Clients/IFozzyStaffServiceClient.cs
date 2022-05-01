using Collecting.Interfaces.Clients.Responses;

namespace Collecting.Interfaces.Clients
{
    public interface IFozzyStaffServiceClient
    {
        Task<GetFozzyStaffResult> GetStaffInfoByGlobalUserId(int globalUserId, CancellationToken cancellationToken);

        Task<GetFozzyStaffResult> GetStaffInfoByInn(string inn, CancellationToken cancellationToken);
    }
}
