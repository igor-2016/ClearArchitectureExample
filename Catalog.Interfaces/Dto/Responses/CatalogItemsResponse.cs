using Catalog.Interfaces.Dto.Requests;
using DomainServices.Interfaces;
using ECom.Entities.Models;
using ECom.Types.EcomCatalog.Entities;
using ECom.Types.EcomCatalog.Responses;
using ECom.Types.ServiceBus;

namespace Catalog.Interfaces.Dto.Responses
{
    public class CatalogItemsResponse : ISimpleCatalogItemsResponse
    {
        public List<SimpleCatalogItem> Items { get; set; }
        public int? ItemsCount { get; set; }
        public List<SkuSetFilialInfo> Filials { get; set; }
        public EComError EComError { get; set; }

        public CatalogInfo GetSimpleCatalogItemsResponse(CatalogItemsRequest request)
        {

            if (EComError.ErrorCode != 0)
            {
                return new CatalogInfo();
            }

            var result =  new CatalogInfo()
            {
               BasketGuid = request.BasketGuid,
               DeliveryType = request.DeliveryType,
               FilialId = request.FilialId,
               LagerId = request.SkuId,
              
                //Merchant = request.Mer  // TODO
            };

            foreach (var item in Items)
            {
                var freezeStatus = item.TechParameters.FirstOrDefault(x => x.Key == "30099"); //ValueId = 3816  
                result.Items.Add(new CatalogItem()
                {
                    Id = item.Id,
                    Unit = item.Unit,
                    Name = item.Name,
                    NameForSite = item.TechParameters.FirstOrDefault(x => x.Name == "Название для сайта / МП")?.Value ?? "Не указано",
                    Price = item.Prices.FirstOrDefault(x => x.Type == "price")?.Value ?? 0,
                    PriceOpt = item.Prices.FirstOrDefault(x => x.Type == "priceOpt")?.Value ?? 0,
                    PriceOld = item.Prices.FirstOrDefault(x => x.Type == "oldPrice")?.Value ?? 0,
                    FreezeStatus = freezeStatus?.Value ?? "Не указан",
                    FreezeStatusId = freezeStatus?.ValueId?.ToIntNull() ?? null,
                    IsWeighted = item.Parameters.Any(x => x.Key == "isWeighted"),
                    //IsActivityEnable  //TODO
                    //IsCollectable //TODO
                    SortingCategory = "Категория сортировки для оптимальной сборки товаров",  //TODO
                });
            }
            return result;
        }
    }
}
