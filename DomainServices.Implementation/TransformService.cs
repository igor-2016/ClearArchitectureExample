using AutoMapper;
using DomainServices.Interfaces;
using DomainServices.Interfaces.Delegates;
using DomainServices.Interfaces.Exceptions;
using ECom.Entities.Models;
using ECom.Types.Collect;
using ECom.Types.Delivery;
using ECom.Types.Orders;
using Entities.Models.Collecting;
using Entities.Models.Expansion;
using Entities.Models.Workflow;
using Utils;
using Utils.Consts;
using Utils.Extensions;

namespace DomainServices.Implementation
{
    public class TransformService : ITransformService
    {

        public const decimal DeltaQtyLessTwo = 0.2m;
        public const decimal DeltaQtyMoreOrEqualTwo = 0.1m;


        private readonly IMapper _mapper;
        private readonly IEntityMappingService _mappingService;
        private readonly IDateTimeService _dateTimeService;

        public TransformService(
          IMapper mapper,
          IEntityMappingService mappingService, 
          IDateTimeService dateTimeService
        )
        {
            _mapper = mapper;
            _mappingService = mappingService;
            _dateTimeService = dateTimeService;
        }

   
        public OrderData ToFozzyOrder(TraceableOrder order)
        {
            var orderData = new OrderData()
            {
                order = new[]
                {
                    new FzShopOrder()
                    {
                        chequePrintDateTime = order.ChequePrintDateTime.ToStringDateTimeNullable(),
                        clientFullName = order.ClientFullName,
                        clientMobilePhone = order.ClientMobilePhone,
                        clientMobilePhoneAlt1 = order.ClientMobilePhoneAlt1,
                        containerBarcodes = order.ContainerBarcodes,

                        contragentFullName = order.ContragentFullName,
                        contragentOKPO = order.ContragentOKPO,
                        dateModified =  order.DateModified.ToStringDateTime(),
                        deliveryAddress = order.DeliveryAddress,
                        deliveryDate = order.DeliveryDate.ToStringDate(),  // NO time!!!!
                                      
                        deliveryId = order.DeliveryId,

                        deliveryTimeFrom = order.DeliveryTimeFrom.ToStringTime(), // time slot 
                        deliveryTimeTo = order.DeliveryTimeTo.ToStringTime(),    // time slot
                        driverId = order.DriverId?.ToString() ?? "",   // игнор          
                        driverName = order.DriverName,
                        filialId = order.FilialId,  //1614,
                        globalUserId = order.UserInn,
                        lastContainerBarcode = order.LastContainerBarcode,
                        logisticsType = order.LogisticsType,
                        megaContainerBarcodes = order.MegaContainerBarcodes,
                        orderBarcode = order.OrderBarcode,
                        orderCreated = order.OrderCreated.ToStringDateTime(),
                        orderFrom = order.OrderFrom.ToString(),
                        orderId = order.OrderNumber,
                        orderStatus = order.OrderStatus.ToString(),
                        paymentId = order.PaymentId.ToString(),
                        placesCount  = order.PlacesCount?.ToString() ?? "",
                        priority  = order.Priority,
                        remark = order.Remark,
                            rroNumber = order.RroNumber,
                        sumPaymentFromInternet = order.SumPaymentFromInternet.ToDecimalString(),
                        sumPaymentFromKassa =  order.SumPaymentFromKassa.ToDecimalString(),

                    }
                }
            };

            var lines = new List<FzShopOrderLines>();

            var defaultRowNum = 1;
            foreach (var lineInfo in order.Items)
            {
                lines.Add(new FzShopOrderLines(order.BasketId, lineInfo.Id, lineInfo.IsWeighted)
                {
                    //AutoReplacements = 118,234,1234,456,2225,5,   // пока не нужно
                    //CustomParams = line.Id.ToString(), // вычисляется в конструкторе

                    IsActivityEnable = lineInfo.IsActivityEnable.ToIntStringNull(),
                    containerBarcode = lineInfo.ContainerBarcode,
                    dateModified = lineInfo.DateModified.ToStringDateTime(),
                    freezeStatus = lineInfo.FreezeStatus ?? 0,
                    globalUserId = lineInfo.UserInn,
                    lagerId = lineInfo.LagerId,
                    lagerName = lineInfo.LagerName,
                    lagerUnit = lineInfo.LagerUnit,
                    orderId = order.ExternalOrderId.HasValue ? order.ExternalOrderId.Value : int.Parse(order.OrderNumber),
                    orderQuantity = lineInfo.OrderQuantity.ToDecimalString(),
                    pickerQuantity = lineInfo.PickerQuantity.ToDecimalString(),
                    priceOut = lineInfo.PriceOut.ToDecimalString(),
                    replacementLagers = lineInfo.ReplacementLagers,

                    rowNum = lineInfo?.RowNum ?? defaultRowNum,
                });
                defaultRowNum++;
            }

            orderData.orderLines = lines.ToArray();

            return orderData;

        }

        public List<AcceptedCollectedItem> ToCollectingItems(TraceableOrder order)
        {
            var collectItems = order
               .Items
               .Select(x => new AcceptedCollectedItem
               {
                   LagerId = x.LagerId,
                   OrderQty = x.OrderQuantity,
                   PickerQty = x.PickerQuantity ?? 0,
                   PickerId = x.GlobalUserId,
                   PickerName = x.PickerName,
                   Collectable = x.Collectable,
                   //ExciseBarcodes = new List<string>(),
                   ReplacementOnLagerId = x.ReplacementOnLagerId,
                   Replacements = x.ReplacementLagers.ToArrayOfInts()
               }).ToList();

            return collectItems;
        }

        //public List<NewAcceptedCollectedItem> ToNewCollectingItems(TraceableOrder order)
        //{
        //    //throw new NotImplementedException();
        //    return _mapper.Map<List<TraceableOrderItem>, List<NewAcceptedCollectedItem>>(order.Items.ToList());
        //}


        //public List<BasketCollectedItem> ToBasketCollectingItems(FozzyCollectableOrderInfo orderInfo)
        //{
        //    var collectItems = orderInfo
        //       .CollectableItems
        //       .Select(x => new BasketCollectedItem
        //       {
        //           LagerId = x.LagerId,
        //           OrderQty = 0,
        //           PickerQty = x.PickerQuantity ?? 0,
        //           PickerId = x.GlobalUserId,
        //           PickerName = x.PickerName,
        //           Collectable = x.Collectable,
        //           ExciseBarcodes = new List<string>(),
        //           ReplacementOnLagerId = x.ReplacementOnLagerId,
        //           Replacements = x.ReplacementLagers.ToArrayOfInts()
        //       }).ToList();

        //    return collectItems;
        //}


        public List<BasketCollectedItem> ToBasketCollectingItems(TraceableOrder order)
        {
            var collectItems = order
               .Items
               .Select(x => new BasketCollectedItem
               {
                   LagerId = x.LagerId,
                   OrderQty = x.OrderQuantity > 0 ? x.OrderQuantity : null,
                   PickerQty = x.PickerQuantity ?? 0,
                   PickerId = x.GlobalUserId,
                   PickerName = x.PickerName,
                   Collectable = x.Collectable,
                   ExciseBarcodes = new List<string>(),
                   ReplacementOnLagerId = x.ReplacementOnLagerId,
                   Replacements = x.ReplacementLagers.ToArrayOfInts()
               }).ToList();

            return collectItems;
        }

        public async Task<TraceableOrder> ToNewTraceableOrder(
            //ICollectableOrderInfo orderInfo,
            FozzyCollectableOrderInfo orderInfo,
            CatalogSingleItemInfoExtractor catalogSingleItemInfoExtractor,
            OrderOriginExtractor orderOriginExtractor,
            RowNumCalculator rowNumCalculator,
            LogisticTypeCalculator logisticTypeCalculator, CancellationToken cancellationToken)
        {
            var linesInfo = orderInfo.CollectableItems;

            //var picker = pickerInfoExtractorByBasketId(orderInfo.BasketGuid);
            var orderOrigin = //orderOriginExtractorByBasketId(orderInfo.BasketGuid) ??
                orderOriginExtractor(orderInfo.MerchantId, orderInfo.FilialId, orderInfo.BasketType) ?? 1;

            var traceableOrder = new TraceableOrder()
            {
                BasketId = orderInfo.BasketGuid,
                ChequePrintDateTime = null, // NONE!
                ClientFullName = orderInfo.CustomerName ?? String.Empty,
                ClientMobilePhone = orderInfo.CustomerPhone ?? String.Empty,
                ClientMobilePhoneAlt1 = orderInfo.CustomerPhoneAlt ?? String.Empty,
                ContainerBarcodes = string.Empty, // NONE!
                ContragentFullName = orderInfo.ContragentFullName ?? String.Empty,
                ContragentOKPO = orderInfo.ContragentOKPO ?? String.Empty,
                DateModified = _dateTimeService.Now,
                    //throw new CollectException(CollectErrors.InvalidDateModified, order.dateModified),
                DeliveryAddress = orderInfo.ClientAddress ?? String.Empty,
                DeliveryDate = orderInfo.PickupDate.Date,
                    //throw new CollectException(CollectErrors.InvalidDeliveryDate, order.deliveryDate),
                DeliveryId = _mappingService.GetDeliveryTypeFromEcomToFozzy((int)orderInfo.DeliveryType)
                  ?? throw new DomainException(DomainErrors.NotFoundMapFromEComToFozzyDeliveryType, 
                        ((int)orderInfo.DeliveryType).ToString()),
                DeliveryTimeFrom = orderInfo.TimeSlotFrom.TimeOfDay,
                DeliveryTimeTo = orderInfo.TimeSlotTo.TimeOfDay,
                DriverId = null, // NONE!
                DriverName = orderInfo.CourierName ?? String.Empty,
                ExternalOrderId = null, // NONE!
                FilialId = orderInfo.FilialId,
                Id = Guid.NewGuid(),
                LastContainerBarcode = string.Empty, // NONE!
                LogisticsType = string.Empty, //смотри ниже
                MegaContainerBarcodes = string.Empty, // NONE!
                OrderBarcode = orderInfo.OrderBarCode ?? string.Empty,
                OrderCreated = orderInfo.Created.Date,
                OrderNumber = orderInfo.OrderNumber,
                OrderStatus = orderInfo.MerchantStateId ?? 0,
                PaymentId = _mappingService.GetPaymentTypeFromEcomToFozzy((int)orderInfo.OrderPaymentType)
                  ?? throw new DomainException(DomainErrors.NotFoundMapFromEComToFozzyPaymentType, 
                    ((int)orderInfo.OrderPaymentType).ToString()),
                PlacesCount = 0,  // NONE!
                Priority = orderInfo.PickupDate.ToPriority2(orderInfo.TimeSlotFrom),
                Remark = orderInfo.Notes ?? String.Empty, 
                RroNumber = string.Empty, // NONE!
                SumPaymentFromInternet = orderInfo.SumOut,
                SumPaymentFromKassa = null,// NONE!
               
                OrderFrom = orderOrigin, 
                GlobalUserId = null,  // NONE!
                UserInn = string.Empty, // NONE!
                MerchantId = orderInfo.MerchantId,
                PickerName =  string.Empty, // NONE!
                
            };

            var catalogItemInfos = new List<CatalogInfo>();
            foreach (var lineInfo in linesInfo)
            {
                var catalogItemInfo = await catalogSingleItemInfoExtractor(lineInfo.LagerId, orderInfo.FilialId, 
                    (Merchant)orderInfo.MerchantId, cancellationToken,  orderInfo.DeliveryType, orderInfo.BasketGuid) ??
                     throw new DomainException(DomainErrors.NotFoundCatalogItem, lineInfo.LagerId.ToString())
                        .AddRequestedTarget(CommonConsts.Subsystem.EComCatalog);
                
                var catalogItem = catalogItemInfo.GetSingleItem() ??
                    throw new DomainException(DomainErrors.NotFoundCatalogItem, lineInfo.LagerId.ToString())
                        .AddRequestedTarget(CommonConsts.Subsystem.EComCatalog);

                //var pickerItem = pickerInfoExtractorByBasketIdAndLagerId(orderInfo.BasketGuid, lineInfo.LagerId)
                 //   ?? picker;

                catalogItemInfos.Add(catalogItemInfo);

                var traceableItem = new TraceableOrderItem(Guid.NewGuid(), traceableOrder.Id, traceableOrder.BasketId,
                   catalogItem.IsWeighted) 
                {
                    ContainerBarcode = string.Empty, // NONE!
                    DateModified = lineInfo.Modified?.Date ?? traceableOrder.DateModified,
                    ExternalOrderId = traceableOrder.ExternalOrderId, // NONE!
                    FreezeStatus = catalogItem.FreezeStatusId,
                    GlobalUserId = null, // NONE!
                    LagerId = lineInfo.LagerId,
                    IsActivityEnable = catalogItem.IsActivityEnable, 
                    LagerName = catalogItem.Name,
                    LagerUnit = catalogItem.Unit,
                    OrderQuantity = lineInfo.Qty,
                    OrderNumber = traceableOrder.OrderNumber,
                    PickerQuantity = null,
                    PriceOut = lineInfo.PriceOut,
                    ReplacementLagers = lineInfo.ReplacementLagers.ToCommaString(),
                    RowNum = 0, // смотри ниже калькулятор
                    UserInn = string.Empty,  // NONE!
                    ReplacementOnLagerId = lineInfo.ReplacementOnLagerId,
                    Collectable = catalogItem.IsCollectable ?? true,
                    OrderId = traceableOrder.Id,
                    PickerName = string.Empty, // NONE!
                    IsFilled = false,
                    //EcomItemId = lineInfo.Id,
                    SortingCategory = catalogItem.SortingCategory ?? string.Empty,
                    
                    //Id                look into constructor
                    //BasketId  look into constructor
                    //CustomParams      look into constructor
                    //IsWeighted  look into constructor
                };
                traceableOrder.Items.Add(traceableItem);
            }

            rowNumCalculator(traceableOrder.Items);

            traceableOrder.LogisticsType = logisticTypeCalculator(catalogItemInfos);

            return traceableOrder;
        }


        /// <summary>
        /// После подтверждения оператором
        /// </summary>
        public async Task<TraceableOrder> FromAcceptedOrder(
            FozzyCollectableOrderInfo orderInfo,
            TraceableOrder currentOrder,
            CatalogSingleItemInfoExtractor catalogSingleItemInfoExtractor,
            RowNumCalculator rowNumCalculator,
            IsFilledCalculator isFilledCalculator,
            LogisticTypeCalculator logisticTypeCalculator, CancellationToken cancellationToken)
        {
            var linesInfo = orderInfo.CollectableItems;

            var orderOrigin = currentOrder.OrderFrom;

            var traceableOrder = new TraceableOrder()
            {
                BasketId = orderInfo.BasketGuid,
                ChequePrintDateTime = currentOrder.ChequePrintDateTime, 
                ClientFullName = orderInfo.CustomerName ?? currentOrder.ClientFullName,
                ClientMobilePhone = orderInfo.CustomerPhone ?? currentOrder.ClientMobilePhone,
                ClientMobilePhoneAlt1 = orderInfo.CustomerPhoneAlt ?? currentOrder.ClientMobilePhoneAlt1,
                ContainerBarcodes = currentOrder.ContainerBarcodes, 
                ContragentFullName = orderInfo.ContragentFullName ?? currentOrder.ContragentFullName,
                ContragentOKPO = orderInfo.ContragentOKPO ?? currentOrder.ContragentOKPO,
                DateModified = _dateTimeService.Now,
                //throw new CollectException(CollectErrors.InvalidDateModified, order.dateModified),
                DeliveryAddress = orderInfo.ClientAddress,
                DeliveryDate = orderInfo.PickupDate.Date,
                //throw new CollectException(CollectErrors.InvalidDeliveryDate, order.deliveryDate),
                DeliveryId = _mappingService.GetDeliveryTypeFromEcomToFozzy((int)orderInfo.DeliveryType)
                  ?? currentOrder.DeliveryId,
                        //throw new DomainException(DomainErrors.NotFoundMapFromEComToFozzyDeliveryType,
                        // ((int)orderInfo.DeliveryType).ToString()),
                DeliveryTimeFrom = orderInfo.TimeSlotFrom.TimeOfDay,
                DeliveryTimeTo = orderInfo.TimeSlotTo.TimeOfDay,
                DriverId = currentOrder.DriverId, 
                DriverName = orderInfo.CourierName ?? currentOrder.DriverName,
                ExternalOrderId = currentOrder.ExternalOrderId,
                FilialId = orderInfo.FilialId,
                Id = currentOrder.Id,
                LastContainerBarcode = currentOrder.LastContainerBarcode, 
                LogisticsType = string.Empty, //смотри ниже
                MegaContainerBarcodes = currentOrder.MegaContainerBarcodes,
                OrderBarcode = orderInfo.OrderBarCode ?? currentOrder.OrderBarcode,
                OrderCreated = orderInfo.Created.Date,
                OrderNumber = orderInfo.OrderNumber,
                OrderStatus = orderInfo.MerchantStateId ?? currentOrder.OrderStatus,
                PaymentId = _mappingService.GetPaymentTypeFromEcomToFozzy((int)orderInfo.OrderPaymentType)
                  ?? throw new DomainException(DomainErrors.NotFoundMapFromEComToFozzyPaymentType,
                    ((int)orderInfo.OrderPaymentType).ToString()),
                PlacesCount = currentOrder.PlacesCount, 
                Priority = orderInfo.PickupDate.ToPriority2(orderInfo.TimeSlotFrom),
                Remark = orderInfo.Notes ?? currentOrder.Remark, 
                RroNumber = currentOrder.RroNumber,
                SumPaymentFromInternet = orderInfo.SumOut, //currentOrder.SumPaymentFromInternet ?
                SumPaymentFromKassa = currentOrder.SumPaymentFromKassa,

                OrderFrom = orderOrigin,
                GlobalUserId = currentOrder.GlobalUserId,
                UserInn = currentOrder.UserInn,
                MerchantId = orderInfo.MerchantId,
                PickerName = currentOrder.PickerName,
                RowVersion = currentOrder.RowVersion,
            };

            var catalogItemInfos = new List<CatalogInfo>();
            foreach (var lineInfo in linesInfo)
            {


                var catalogItemInfo = await catalogSingleItemInfoExtractor(lineInfo.LagerId, orderInfo.FilialId,
                   (Merchant)orderInfo.MerchantId, cancellationToken, orderInfo.DeliveryType, orderInfo.BasketGuid) ??
                    throw new DomainException(DomainErrors.NotFoundCatalogItem, lineInfo.LagerId.ToString())
                        .AddRequestedTarget(CommonConsts.Subsystem.EComCatalog);

                var catalogItem = catalogItemInfo.GetSingleItem() ??
                    throw new DomainException(DomainErrors.NotFoundCatalogItem, lineInfo.LagerId.ToString())
                        .AddRequestedTarget(CommonConsts.Subsystem.EComCatalog);

                //var pickerItem = pickerInfoExtractorByBasketIdAndLagerId(orderInfo.BasketGuid, lineInfo.LagerId);

                // read from current line if exists
                var currentTraceableItem = currentOrder.Items.FirstOrDefault(x => x.LagerId == lineInfo.LagerId);

                catalogItemInfos.Add(catalogItemInfo);

                var traceableItem = new TraceableOrderItem(currentTraceableItem?.Id ?? Guid.NewGuid(), // or create new one 
                    traceableOrder.Id, traceableOrder.BasketId, catalogItem.IsWeighted)
                {
                    ContainerBarcode = currentTraceableItem?.ContainerBarcode ?? string.Empty, 
                    DateModified = lineInfo.Modified?.Date ?? currentTraceableItem?.DateModified ?? traceableOrder.DateModified,
                    ExternalOrderId = traceableOrder.ExternalOrderId,
                    FreezeStatus = catalogItem.FreezeStatusId,  // refresh from catalog
                    GlobalUserId = currentTraceableItem?.GlobalUserId, //?? pickerItem?.Id,
                    LagerId = lineInfo.LagerId,
                    IsActivityEnable = catalogItem.IsActivityEnable, // refresh from catalog
                    LagerName = catalogItem.Name, // refresh from catalog
                    LagerUnit = catalogItem.Unit, // refresh from catalog
                    OrderQuantity = lineInfo.Qty,
                    OrderNumber = traceableOrder.OrderNumber,
                    PickerQuantity = currentTraceableItem?.PickerQuantity ?? null,
                    PriceOut = lineInfo.PriceOut,
                    ReplacementLagers = currentTraceableItem?.ReplacementLagers ?? lineInfo.ReplacementLagers.ToCommaString(),
                    RowNum = currentTraceableItem?.RowNum ?? 0, // смотри ниже калькулятор
                    UserInn = currentTraceableItem?.UserInn ?? string.Empty,
                    ReplacementOnLagerId = currentTraceableItem?.ReplacementOnLagerId ?? lineInfo.ReplacementOnLagerId,
                    Collectable = catalogItem.IsCollectable ?? currentTraceableItem?.Collectable ?? true, // refresh from catalog
                    OrderId = traceableOrder.Id,
                    PickerName = currentTraceableItem?.PickerName ?? string.Empty,
                    IsFilled = false, //  смотри ниже калькулятор
                    //EcomItemId = currentTraceableItem?.EcomItemId ?? lineInfo.Id,
                    SortingCategory = catalogItem.SortingCategory ?? string.Empty, // refresh from catalog
                    //Id                look into constructor
                    //BasketId  look into constructor
                    //CustomParams      look into constructor
                    //IsWeighted  look into constructor
                };
                traceableOrder.Items.Add(traceableItem);
            }

            // is filled before row num calculator!!!
            isFilledCalculator(traceableOrder.Items);

            rowNumCalculator(traceableOrder.Items);

            traceableOrder.LogisticsType = logisticTypeCalculator(catalogItemInfos);

            return traceableOrder;
        }


        public TraceableOrderItem FromCatalogItem(TraceableOrder order, CatalogItem catalogItem)
        {
            return new TraceableOrderItem(Guid.NewGuid(), order.Id, order.BasketId, catalogItem.IsWeighted)
            {
                //BasketId
                Collectable = catalogItem.IsCollectable ?? true,
                DateModified = _dateTimeService.Now,
                FreezeStatus = catalogItem.FreezeStatusId,
                GlobalUserId = order.GlobalUserId,
                IsActivityEnable = catalogItem.IsActivityEnable,
                IsFilled    = false,
                LagerId = catalogItem.Id,
                LagerName = catalogItem.Name,
                OrderNumber = order.OrderNumber,
                LagerUnit   = catalogItem.Unit,
                OrderQuantity = 0,
                PriceOut = catalogItem.Price,
                PickerName= order.PickerName,
                PickerQuantity = null,
                //ReplacementLagers = null,
                //ReplacementOnLagerId
                UserInn = order.UserInn,
                RowNum = 0,
                ExternalOrderId = order.ExternalOrderId,
                //IsWeighted
                //OrderId
                SortingCategory = catalogItem.SortingCategory,
                
                ContainerBarcode = string.Empty, // bug fixed
                ReplacementLagers = String.Empty,
                

            };

        }

        public string GetLogisticType(IEnumerable<CatalogInfo> catalogInfos)
        {
            var set = new HashSet<string>();
            foreach (var catalogInfo in catalogInfos)
            {
                set.Add(catalogInfo?.GetSingleItem()?.FreezeStatus ?? "неизвестно");
            }
            return set.ToVerLineString();

        }

        private async Task<Picker> GetUpdatedPicker(Picker currentPicker, string changedPickerInn, 
            PickerInfoExtractorByInn pickerInfoExtractorByInn, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(changedPickerInn) && currentPicker.Inn != changedPickerInn)
            {
                return await pickerInfoExtractorByInn(changedPickerInn, cancellationToken);
            }

            return currentPicker;
        }

        // TODO review currentTraceableOrder как и что перекрывается !!
        /// <summary>
        /// Просто берем всё из Фоззи ордера и формируем изменённый для дальнейшего сравнения с текущим Traceable Order
        /// </summary>
        /// <param name="currentTraceableOrder"></param>
        /// <param name="currentPicker"></param>
        /// <param name="orderData"></param>
        /// <param name="pickerInfoExtractorByInn"></param>
        /// <returns></returns>
        /// <exception cref="DomainException"></exception>
        public async Task<TraceableOrder> FromFozzyOrder(
            TraceableOrder currentTraceableOrder,
            Picker currentPicker, 
            OrderData orderData,
            CatalogSingleItemInfoExtractor catalogSingleItemInfoExtractor,
            PickerInfoExtractorByInn pickerInfoExtractorByInn,
            IsFilledCalculator isFilledCalculator,
            RowNumCalculator rowNumCalculator,
            LogisticTypeCalculator logisticTypeCalculator, 
            CancellationToken cancellationToken)
        {
            var fozzyOrder = orderData.GetOrder();
            var fozzyLines = orderData.orderLines;

            currentPicker = //pickerInfoExtractorByBasketId(currentTraceableOrder.BasketId) ?? 
                await GetUpdatedPicker(currentPicker, fozzyOrder.globalUserId, pickerInfoExtractorByInn, cancellationToken)
                    ?? throw new DomainException(DomainErrors.NotFoundPicker, fozzyOrder.globalUserId);

            
            var traceableOrder = new TraceableOrder()
            {
                BasketId = currentTraceableOrder.BasketId,
                ChequePrintDateTime = fozzyOrder.chequePrintDateTime.ToDateTime() ?? 
                   currentTraceableOrder.ChequePrintDateTime,
                ClientFullName = fozzyOrder.clientFullName,
                ClientMobilePhone = fozzyOrder.clientMobilePhone,
                ClientMobilePhoneAlt1 = fozzyOrder.clientMobilePhoneAlt1,
                ContainerBarcodes = fozzyOrder.containerBarcodes ?? string.Empty,
                ContragentFullName = fozzyOrder.contragentFullName,
                ContragentOKPO = fozzyOrder.contragentOKPO,
                DateModified = fozzyOrder.dateModified.ToDateTime() ?? currentTraceableOrder.DateModified, 
                    //throw new DomainException(DomainErrors.InvalidDateModified, fozzyOrder.dateModified),
                DeliveryAddress = fozzyOrder.deliveryAddress,
                DeliveryDate = fozzyOrder.deliveryDate.ToDateOnly() ?? currentTraceableOrder.DeliveryDate, // Date Only???
                    //throw new DomainException(DomainErrors.InvalidDeliveryDate, fozzyOrder.deliveryDate),
                DeliveryId = fozzyOrder.deliveryId,
                DeliveryTimeFrom = fozzyOrder.deliveryTimeFrom.ToTime() ?? currentTraceableOrder.DeliveryTimeFrom,
                    //throw new DomainException(DomainErrors.InvalidDeliveryTimeFrom, fozzyOrder.deliveryTimeFrom),
                DeliveryTimeTo = fozzyOrder.deliveryTimeTo.ToTime() ?? currentTraceableOrder.DeliveryTimeTo,
                    //throw new DomainException(DomainErrors.InvalidDeliveryTimeTo, fozzyOrder.deliveryTimeTo),
                DriverId = fozzyOrder.driverId.ToIntNulable(),
                DriverName = fozzyOrder.driverName,
                ExternalOrderId = fozzyOrder.orderId.ToIntNulable() ?? currentTraceableOrder.ExternalOrderId ??
                    throw new DomainException(DomainErrors.InvalidOrderIdAsInt, fozzyOrder.orderId),
                FilialId = fozzyOrder.filialId,
                Id = currentTraceableOrder.Id,
                LastContainerBarcode = fozzyOrder.lastContainerBarcode,
                LogisticsType = fozzyOrder.logisticsType,  // or recalculate???
                MegaContainerBarcodes = fozzyOrder.megaContainerBarcodes,
                OrderBarcode = fozzyOrder.orderBarcode,
                OrderCreated = fozzyOrder.orderCreated.ToDateTime() ?? currentTraceableOrder.OrderCreated,
                    //throw new DomainException(DomainErrors.InvalidCreatedOrder, fozzyOrder.orderCreated),
                OrderNumber = fozzyOrder.orderId,
                OrderStatus = fozzyOrder.orderStatus.ToInt(),
                PaymentId = fozzyOrder.paymentId.ToInt(),
                PlacesCount = fozzyOrder.placesCount.ToInt(),
                Priority = fozzyOrder.priority,
                Remark = fozzyOrder.remark,
                RroNumber = fozzyOrder.rroNumber,
                SumPaymentFromInternet = fozzyOrder.sumPaymentFromInternet.ToDecimalNull(),
                SumPaymentFromKassa = fozzyOrder.sumPaymentFromKassa.ToDecimalNull(),

                OrderFrom = fozzyOrder.orderFrom.ToInt(),
                GlobalUserId = currentPicker.Id.NullFromIntZero(), 
                UserInn = currentPicker.Inn ,
                MerchantId = currentTraceableOrder.MerchantId,
                RowVersion = currentTraceableOrder.RowVersion,
                PickerName = currentPicker.Name,
                
                
            };

            var ecomDeliveryType = _mappingService.GetDeliveryTypeFromFozzyToEcom((int)currentTraceableOrder.DeliveryId)
                  ?? throw new DomainException(DomainErrors.NotFoundMapFromFozzyToEComDeliveryType,
                        ((int)currentTraceableOrder.DeliveryId).ToString());

            var catalogItemInfos = new List<CatalogInfo>();


            foreach (var fozzyLine in fozzyLines)
            {
                var catalogItemInfo = await catalogSingleItemInfoExtractor(fozzyLine.lagerId, currentTraceableOrder.FilialId,
                  (Merchant)currentTraceableOrder.MerchantId, cancellationToken, (DeliveryType)ecomDeliveryType, 
                        currentTraceableOrder.BasketId) ??
                   throw new DomainException(DomainErrors.NotFoundCatalogItem, fozzyLine.lagerId.ToString())
                      .AddRequestedTarget(CommonConsts.Subsystem.EComCatalog);

                var catalogItem = catalogItemInfo.GetSingleItem() ??
                    throw new DomainException(DomainErrors.NotFoundCatalogItem, fozzyLine.lagerId.ToString())
                        .AddRequestedTarget(CommonConsts.Subsystem.EComCatalog);

                catalogItemInfos.Add(catalogItemInfo);

                var currentLinePicker = await GetUpdatedPicker(currentPicker, fozzyLine.globalUserId, 
                    pickerInfoExtractorByInn, cancellationToken)
                    ?? throw new DomainException(DomainErrors.NotFoundPickerInLine, fozzyLine.globalUserId);

                var currentTraceableItem = currentTraceableOrder.Items.FirstOrDefault(x => x.Id == fozzyLine.GetLineId());

                var traceableItem = new TraceableOrderItem()
                {
                    BasketId = traceableOrder.BasketId,
                    ContainerBarcode = fozzyLine.containerBarcode ?? string.Empty,
                    CustomParams = fozzyLine?.CustomParams ?? string.Empty,
                    DateModified = fozzyLine?.dateModified.ToDateTime() ??
                        throw new DomainException(DomainErrors.InvalidDateModifiedLine, fozzyLine?.dateModified ?? string.Empty),
                    ExternalOrderId = traceableOrder.ExternalOrderId,
                    FreezeStatus = catalogItem?.FreezeStatusId ?? fozzyLine.freezeStatus,
                    Id = fozzyLine.GetLineId() ?? Guid.NewGuid(),
                    GlobalUserId = currentLinePicker.Id.NullFromIntZero(), 
                    LagerId = fozzyLine.lagerId,
                    IsActivityEnable = catalogItem?.IsActivityEnable ?? (fozzyLine?.IsActivityEnable ?? "0").ToBool(),
                    LagerName = catalogItem?.NameForSite ?? fozzyLine?.lagerName ?? string.Empty,
                    LagerUnit = catalogItem?.Unit ?? fozzyLine?.lagerUnit ?? string.Empty,
                    OrderQuantity = (fozzyLine?.orderQuantity ?? "0").ToDecimal(),
                    OrderNumber = traceableOrder.OrderNumber,
                    PickerQuantity = (fozzyLine?.pickerQuantity ?? "").ToDecimalNull(),
                    PriceOut = (fozzyLine?.priceOut ?? "0").ToDecimal(),
                    ReplacementLagers = fozzyLine?.replacementLagers ?? string.Empty,
                    RowNum = fozzyLine?.rowNum ?? 0, // or recalculate?
                    UserInn = currentLinePicker.Inn,
                    PickerName = currentLinePicker.Name,
                    OrderId = currentTraceableOrder.Id,
                    Collectable = catalogItem?.IsCollectable ?? true, 
                    IsWeighted = fozzyLine?.GetIsWeighted() ?? catalogItem?.IsWeighted ?? new bool?(), 
                    //EcomItemId = currentTraceableItem?.EcomItemId,
                    SortingCategory = catalogItem?.SortingCategory ?? currentTraceableItem?.SortingCategory ?? string.Empty,
                    IsFilled = false, // смотри ниже
                    //EcomItemId  
                    ReplacementOnLagerId = currentTraceableItem?.ReplacementOnLagerId,
                    
                    //IsFilled  // TODO calc
                };
                traceableOrder.Items.Add(traceableItem);
            }

            isFilledCalculator(traceableOrder.Items);

            rowNumCalculator(traceableOrder.Items);

            traceableOrder.LogisticsType = logisticTypeCalculator(catalogItemInfos);

            return traceableOrder;
        }

        public Task<TraceableOrderItem> CreateNewFromCurrentMinimal(int lagerId,
            TraceableOrderItem template, IDateTimeService dateTimeService, CancellationToken cancellationToken)
        {
            return Task.FromResult(new TraceableOrderItem(
                Guid.NewGuid(), 
                template.OrderId,
                template.BasketId, 
                template.IsWeighted)
            {
                LagerId = lagerId,
                DateModified = dateTimeService.Now,
                OrderNumber = template.OrderNumber,
                ExternalOrderId = template.ExternalOrderId,
                GlobalUserId = template.GlobalUserId,
                PickerName = template.PickerName,
                UserInn = template.UserInn,
            });
        }


        public bool ItemIsFilled(TraceableOrder order, TraceableOrderItem item)
        {
            var alreadyReplaced = order.Items.Any(x => x.ReplacementOnLagerId == item.LagerId);
            return IsFilled(item.OrderQuantity, item.PickerQuantity ?? 0, item.IsWeighted ?? true, alreadyReplaced);
        }

        private static bool IsFilled(decimal qty, decimal qtyPicked, bool isWeightGoods, bool alreadyReplaced)
        {

            if (alreadyReplaced)
                return true;

            if (qtyPicked == 0)
                return false;

            if (qty == qtyPicked)
                return true;

            if (isWeightGoods)
            {
                if (qty < 2)
                {
                    var result = Math.Abs(qty - qtyPicked) <= (DeltaQtyLessTwo * qty);
                    return result;
                }
                else // >= 2
                {
                    var result = Math.Abs(qty - qtyPicked) <= (DeltaQtyMoreOrEqualTwo * qty);
                    return result;
                }
            }
            else
            {
                return qtyPicked >= qty;
            }
        }

    }
}