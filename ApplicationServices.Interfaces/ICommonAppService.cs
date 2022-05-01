using ApplicationServices.Interfaces.Models;
using ApplicationServices.Interfaces.Requests;
using Entities.Models.Expansion;

namespace ApplicationServices.Interfaces
{
    public interface ICommonAppService
    {
      

        /// <summary>
        /// call workflow
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        Task StartCollecting(int oldOrderStatusId, TraceableOrder order,
            CancellationToken cancellationToken);

        /// <summary>
        /// call workflow
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        Task UpdateCollecting(int oldOrderStatusId, TraceableOrder order,
            CancellationToken cancellationToken);

        /// <summary>
        /// call workflow
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        Task DoneCollecting(int oldOrderStatusId, TraceableOrder order,
            CancellationToken cancellationToken);

        /// <summary>
        /// Оповестить ECom.Workflow о событии изменения ордера, которое не обрабатывает ECom.Expansion
        /// </summary>
        /// <param name="oldOrderStatusId"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        Task DefaultAction(int oldOrderStatusId, TraceableOrder order,
            CancellationToken cancellationToken);

        /// <summary>
        /// Вызывается с ТСД при завершении сборки
        /// </summary>
        /// <param name="input"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<TraceableOrder> AcceptCollecting(ICollectableOrdersInput input,
            CancellationToken cancellationToken);

        /// <summary>
        /// Вызывается по умолчанию, если нет обработки в Expansion
        /// </summary>
        /// <param name="input"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<TraceableOrder> ResearchOrder(ICollectableOrdersInput input,
            CancellationToken cancellationToken);

        /// <summary>
        /// Вызывается при предложении замен сборкой Fozzy 
        /// </summary>
        /// <param name="orderToBeUpdated"></param>
        /// <param name="fromFozzyChangedOrder"></param>
        /// <returns></returns>
        Task<TraceableOrder> OfferReplacements(ICollectableOrdersInput input,
            CancellationToken cancellationToken);


        /// <summary>
        /// Вызывается при подтверждении замен оператором
        /// </summary>
        /// <param name="input"></param>
        /// <param name="calcReplacements"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<TraceableOrder> AcceptReplacements(ICollectableOrdersInput input, 
            CancellationToken cancellationToken);


        Task NotifyWorkflowStatusChanged(Guid basketId, int oldOrderStatusId, int newOrderStatusId,
            CancellationToken cancellationToken);

        #region Order Management

        /// <summary>
        /// Создать ордер
        /// </summary>
        /// <param name="newOrder"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<TraceableOrder> CreateOrder(TraceableOrder newOrder, CancellationToken cancellationToken);

        /// <summary>
        /// Получить ордер без позиций
        /// </summary>
        /// <param name="basketId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<TraceableOrder> GetOrderOnlyByBasketId(Guid basketId, CancellationToken cancellationToken);

        /// <summary>
        /// Получить ордер и позиции заказа
        /// </summary>
        /// <param name="basketId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<TraceableOrder> GetOrderWithItemsByBasketId(Guid basketId, CancellationToken cancellationToken);

        /// <summary>
        /// сохраним ордер и только сам ордер (TO DO перенести сюда ещё вызовы с CollectService!!!)
        /// </summary>
        /// <param name="order"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<TraceableOrder> UpdateOrderOnly(TraceableOrder order, CancellationToken cancellationToken);

        /// <summary>
        /// Сохраним ордер  и его подробности (Items)
        /// </summary>
        /// <param name="order"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<TraceableOrder> UpdateOrderAndItems(TraceableOrder order, CancellationToken cancellationToken);


        /// <summary>
        /// Превращает ордер в вид ордера
        /// </summary>
        /// <param name="order"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        TraceableOrderView ToView(TraceableOrder order);


        #endregion Order Management

    }
}