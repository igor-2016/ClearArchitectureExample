using ApplicationServices.Interfaces;
using ApplicationServices.Interfaces.Requests;
using Catalog.Interfaces;
using Collecting.Interfaces;
using DomainServices.Interfaces;
using ECom.Types.Exceptions;
using Entities.Models.Expansion;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace ApplicationServices.Implementation.EventHandlers
{

    public class LookupOrderItem : IOrderProcessEvent<ILookupItemRequest>
    {
        public TraceableOrder Order { get; set; }
        public ILookupItemRequest Data { get; set; }
    }

    public class LookupOrderItemByLagerIdHandler
    {
        private readonly ILogger _logger;
        private readonly ICatalogService _catalogService;
        private readonly ITransformService _transformService;


        public LookupOrderItemByLagerIdHandler(
            ILogger logger,
            ICatalogService catalogService,
            ITransformService transformService
            )
        {
            _logger = logger;
            _catalogService = catalogService;
            _transformService = transformService;
        }


        public async Task<List<TraceableOrderItem>> Handle(LookupOrderItem request,
            CancellationToken cancellationToken = default)
        {
            var data = request.Data;
            var order = request.Order;


            _logger.LogInformation("{msg} {data}", "Lookup item", data);

            Stopwatch sw = Stopwatch.StartNew();
            try
            {

                if (order == null)
                    throw new CollectException(CollectErrors.OrderNotFound, "Ордер не найден!");


                var catalogItem = await _catalogService.GetCatalogItem(order, data.LagerId, cancellationToken);

                var items = await new ExtractItemsByLagerIdOrEComItemIdHandler().Handle(
                    new ExtractItemsEvent()
                    {
                        Order = order,
                        Data = new ExtractData(catalogItem, null)
                    });


                if (items.Count == 0)
                {
                    if (data.ItemId.HasValue)
                        throw new CollectException(CollectErrors.SkuNotFoundInProductSet, "Артикул отсутствует в наборе");

                    // создаём новый, если не найдено в корзине                      
                    var newItem = _transformService.FromCatalogItem(order, catalogItem);
                    items.Add(newItem);
                }

                return items;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{msg} {error} {StackTrace}", "Error occurred while LookupItem", ex.FullMessage(), ex.StackTrace);
                throw;
            }
            finally
            {
                sw.Stop();
            }

        }
    }
}
