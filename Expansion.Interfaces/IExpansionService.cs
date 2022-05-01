using ApplicationServices.Interfaces.Models;
using ECom.Entities.Models;
using Expansion.Interfaces.Dto;

namespace Expansion.Interfaces
{
    public interface IExpansionService
    {
        /// <summary>
        /// 1) Создаёт ТЕСТОВЫЙ ордер в ECom,
        /// 2) Пробрасывает ECom ордер в Экспансию    
        /// 3) Отправляет ордер в Collect.Fozzy на сборку
        /// 4) Начинает сборку 
        /// 5) Собирает всё
        /// 6) Заканчивает сборку
        /// </summary>
        /// <returns></returns>
        Task<CreateOrderAndCollectResponse> DoTestScenarioCreateOrderAndCollectWithReplacements(CreateOrderAndCollectRequest request,
            CancellationToken cancellationToken);

        //Task<TraceableOrderView> EnterCollecting(CollectableOrderInfo ecomToExpansionOrder, 
        //    CancellationToken cancellationToken);
    }
}