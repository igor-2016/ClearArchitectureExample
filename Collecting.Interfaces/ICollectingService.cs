using Entities.Models.Collecting;
using Entities.Models.Expansion;

namespace Collecting.Interfaces
{
    public interface ICollectingService
    {
        Task<TraceableOrder> UpdateOrder(TraceableOrder order, CancellationToken cancellationToken);

        Task<TraceableOrder> GetOrderByOrderId(string orderId, CancellationToken cancellationToken);
        Task<TraceableOrder> GetOrderByExternalOrderId(int orderId, CancellationToken cancellationToken);
        
        Task<TraceableOrder> GetOrderByOrderNumber(string orderNumber, CancellationToken cancellationToken);
        Task<TraceableOrder> GetOrderByBasketId(Guid basketId, CancellationToken cancellationToken);

        Task<Picker> GetFozzyPickerById(int globalUserId, CancellationToken cancellationToken);

        Task<Picker> GetFozzyPickerByInn(string inn, CancellationToken cancellationToken);

        Task SaveFozzyOrder(OrderData orderData, CancellationToken cancellationToken);
        Task<OrderData> GetFozzyOrder(string orderId, CancellationToken cancellationToken);

        /// <summary>
        /// Вызывается из ECom.Workflow, просто обновление сервиса сборки
        /// </summary>
        /// <param name="order"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<OrderData> SendOrderToFozzyWebService(TraceableOrder order, CancellationToken cancellationToken);

        /// <summary>
        /// Вызывается из ECom.Workflow, просто обновление сервиса сборки
        /// </summary>
        /// <param name="order"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<OrderData> SendOrderToFozzyWebSite(TraceableOrder order, CancellationToken cancellationToken);
    }
}