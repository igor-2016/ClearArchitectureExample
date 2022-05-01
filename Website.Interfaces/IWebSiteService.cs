using Entities.Models.Collecting;
using Entities.Models.Expansion;

namespace WebSiteService.Interfaces
{
    public interface IWebSiteService
    {
        /// <summary>
        /// Вызывается из ECom.Workflow, просто обновление сайте ИМ
        /// </summary>
        /// <param name="traceableOrder"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<OrderData> SendOrderToFozzyWebSite(TraceableOrder traceableOrder, CancellationToken cancellationToken);
    }
}