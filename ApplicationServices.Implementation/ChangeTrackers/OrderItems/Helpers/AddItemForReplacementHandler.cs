using ApplicationServices.Interfaces;
using Catalog.Interfaces;
using Collecting.Interfaces;
using DomainServices.Interfaces;
using Entities.Models.Expansion;
using Microsoft.Extensions.Logging;

namespace ApplicationServices.Implementation.EventHandlers
{

    public class AddItemForRepData
    {
        public int LagerId { get; set; }
        public int ReplacementLagerId { get; set; }
    }

    public class AddItemForReplacementEvent : IOrderProcessEvent<AddItemForRepData>
    {
        public TraceableOrder Order { get; set; }
        public AddItemForRepData Data { get; set; }
    }

    public class AddItemForReplacementHandler
    {
        private readonly ILogger _logger;
        private readonly ICatalogService _catalogService;
        private readonly ITransformService _tranformService;

        public AddItemForReplacementHandler(
            ILogger logger,
            ICatalogService catalogService,
            ITransformService tranformService)
        {
            _logger = logger;
            _catalogService = catalogService;
            _tranformService = tranformService;
        }

        public async Task<TraceableOrder> Handle(AddItemForReplacementEvent request,
            CancellationToken cancellationToken = default)
        {
            var data = request.Data;
            var order = request.Order;


            //_logger.LogInformation("{msg} {data}", FozzyConsts.EventHandlerInfo.AddItemFromReplacementsEventHandlerName, request);


   

            var items = await new LookupOrderItemByLagerIdHandler(_logger, _catalogService, _tranformService)
                .Handle(new LookupOrderItem()
                {
                    Order = order,
                    Data = new LookupItemRequest
                    {
                        LagerId = data.LagerId,
                    }
                }, cancellationToken);

            if (items.Count == 0)
                throw new CollectException(CollectErrors.SKUNotFound, "Товар не найден");

            if (items.Count > 1)
                throw new CollectException(CollectErrors.MultipleSKUFound, "найдено более 1 позиции товара");

            var item = items.First();



            //item.PickerQuantity = data.Qty.Value;

            //item.SumOut = item.PriceOut * item.PickerQty.Value;
            //item.SumRozn = item.PriceRozn * item.PickerQty.Value;

            // replacements
            item.ReplacementOnLagerId = data.ReplacementLagerId;
            //item.ReplacementLagers = data.Replacements.ToCommaString();

            order.Items.Add(item);

            //_logger.LogInformation("{msg} {data}", FozzyConsts.EventHandlerInfo.AddItemFromReplacementsEventHandlerName + " After",
            //    item.ReplacementLagers);

            return order;

        }
    }
}
