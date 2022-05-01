using ApplicationServices.Interfaces.Requests;
using Catalog.Interfaces;
using Catalog.Interfaces.Dto;
using Catalog.Interfaces.Enums;
using Collecting.Interfaces;
using DomainServices.Interfaces;
using ECom.Types.Exceptions;
using Entities.Models.Expansion;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Utils;
using Utils.Attributes;

namespace Collecting.UseCases
{
    [ApmTrace]
    public class LookupItemCommand : IRequest<List<TraceableOrderItem>>
    {
        public Guid BasketGuid { get; set; }
        public ILookupItemRequest Request { get; set; }
    }

    public class LookupItemCommandHandler : IRequestHandler<LookupItemCommand, List<TraceableOrderItem>>
    {
        private readonly ILogger _logger;
        //private readonly ICatalogService _catalogService;
        private readonly IMediator _mediator;

        private readonly IEntityMappingService _entityMappingService;

        public LookupItemCommandHandler(
            ILogger<LookupItemCommandHandler> logger,
            //ICatalogService catalogService,
            IEntityMappingService entityMappingService,
            IMediator mediator)
        {
            _logger = logger;
            //_mw = mw;
            _mediator = mediator;
            //_catalogService = catalogService;
            _entityMappingService = entityMappingService;
        }

        public async Task<List<TraceableOrderItem>> Handle(LookupItemCommand request, CancellationToken cancellationToken)
        {
            var data = request.Request;

            using (_logger.LoggingScope(request.BasketGuid))
            {
                _logger.LogInformation("{msg} {data}", "Lookup item", data);

                Stopwatch sw = Stopwatch.StartNew();
                try
                {
                    //if (data.Args == null)
                    //    throw new CollectException(CollectErrors.ArgumentNull, "request is null");

                    if (string.IsNullOrEmpty(data.Barcode) && !data.LagerId.HasValue)
                        throw new CollectException(CollectErrors.BarcodeOrLagerIdRequired, "необходимо указать либо штрихкод либо артикул");

                    if (!string.IsNullOrEmpty(data.Barcode) && data.LagerId.HasValue)
                        throw new CollectException(CollectErrors.BarcodeOrLagerIdRequired, "необходимо указать либо штрихкод либо артикул");

                    #region чтение заказа и поиск артикула в SKU Service
                    var view = await _mediator.Send(new LoadCollectOrderCommand()
                    {
                        BasketGuid = request.BasketGuid,
                    });

                    if (data.ItemId.HasValue)
                    {
                        var scopeItem = view.Items.FirstOrDefault(x => x.Id == data.ItemId.Value);

                        if (scopeItem == null)
                            throw new CollectException(CollectErrors.ProductSetNotFound, "Набор не найден");

                        //if (scopeItem.ParentId.HasValue)
                        //    throw new CollectException(CollectErrors.ProductSetNotFound, "Набор не найден");
                    }


                    //var swData = Stopwatch.StartNew();
                    CatalogResponse gdr = null;
                    try
                    {
                        

                        var ecomDeliveryTypeId =
                            _entityMappingService.GetDeliveryTypeFromFozzyToEcom(view.DeliveryId) ??
                                throw new CollectException(CollectErrors.CannotMapDeliveryTypeFromFozzyToEcom, $"{view.DeliveryId}");
                        
                        /*
                        gdr = await _catalogService.GetGoodsData(
                            new LookupItemRequest() 
                            { 
                                LagerId = data.LagerId, 
                                Barcode = data.Barcode, 
                                ItemId = data.ItemId,
                                FilialId = view.FilialId,
                                BasketGuid = request.BasketGuid,
                                DeliveryTypeId = ecomDeliveryTypeId,

                            }
                            );
                        */
                    }
                    catch (Exception ex)
                    {
                        throw new CollectException(CollectErrors.SKUNotFound, ex.Message);
                    }
                    //swData.Stop();
                    //_mw.Observe("lookup item - waiting data", sw.ElapsedMilliseconds);
                    #endregion

                    #region поиск элемента в корзине

                    if (gdr.Mode == CatalogGoodsDataRequestMode.Discount)
                        throw new CollectException(CollectErrors.WrongBarcode_discount, "Ш/К скидки");

                    if (gdr.Mode == CatalogGoodsDataRequestMode.MultiSKU)
                        throw new CollectException(CollectErrors.WrongBarcode_multisku, "multisku");

                    if (gdr.GoodsData == null)
                        throw new CollectException(CollectErrors.WrongGoodsData, "GoodsData не определена");

                    var lagerId = gdr.GoodsData.LagerId;

                    if (lagerId == 0)
                        throw new CollectException(CollectErrors.WrongGoodsData, "lagerId == 0");

                    
                    var items = await _mediator.Send(new ExtractItemsCommand()
                    {
                        Order = view,
                        GoodsDataRequest = gdr,
                        Request = data,
                    });
                    
                    /*
                    if (items.Count == 0)
                    {
                        if (data.Args.ItemId.HasValue)
                            throw new CollectException(CollectErrors.SkuNotFoundInProductSet, "Артикул отсутствует в наборе");

                        // создаём новый, если не найдено в корзине (только в корне корзины)                      
                        var newItem = new FzShopCollectItemView(gdr.GoodsData, 0);
                        newItem.Id = Guid.Empty;
                        items.Add(newItem);
                    }
                    else
                    {
                        foreach (var item in items)
                        {
                            item.TypeId = gdr.GoodsData.TypeId;
                            item.StoreKolvo = gdr.GoodsData.StoreKolvo;
                        }
                    }
                    */
                    #endregion

                    decimal? itemQty = 1;
                    _logger.LogInformation("{msg} {barcode} {qty} {data}",
                        $"order item found",
                        data.Barcode,
                        itemQty, data);

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
                    //_mw.Observe("lookup item", sw.ElapsedMilliseconds);
                }
            }
        }
    }
}
