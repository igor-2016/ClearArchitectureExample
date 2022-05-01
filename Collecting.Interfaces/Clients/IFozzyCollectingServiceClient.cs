using Collecting.Interfaces.Clients.Responses;

namespace Collecting.Interfaces.Clients
{
    public interface IFozzyCollectingServiceClient
    {
        Task<PutFozzyOrderDataResult> PutOrderData(FozzyOrderData orderData, CancellationToken cancellationToken);
        Task<GetFozzyOrderDataResult> GetOrderData(string orderId, CancellationToken cancellationToken);
    }
}
