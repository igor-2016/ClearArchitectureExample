using Collecting.Controllers.Filters.Exceptions;
using Collecting.Controllers.Responses;
using Collecting.Interfaces;
using Collecting.UseCases;
using DomainServices.Interfaces;
using Entities.Consts;
using Entities.Models.Collecting;
using Entities.Models.Expansion;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using Utils;

namespace Collecting.Controllers
{
    [ApiController]
    [ApiExplorerSettings(GroupName = ExpansionConsts.Common.App.Groups.FozzyCollecting.GroupNameVersionOne)]
    [Route(ExpansionConsts.Common.App.Controllers.Collecting.CollectingBaseRoute)]
    [SwaggerTag(ExpansionConsts.Common.App.Controllers.Collecting.Description)]
    public class CollectingController : ControllerBase
    {
        private readonly ILogger<CollectingController> _logger; 
        public CollectingController(ILogger<CollectingController> logger)
        {
            _logger = logger;
        }


        /// <summary>
        /// Начинает сборку, предлагает замены, завершает сборку (другие изменения по ордеру)
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Consumes("application/xml")]
        [Produces("application/xml")]
        [HttpPost(ExpansionConsts.Common.App.Controllers.Collecting.ChangeOrderRoute)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ConfirmResponse))]
        [CollectingExceptionHandler]
        public async Task<ConfirmResponse> ChangeCollecting(
            [FromBody] OrderData request,
            [FromServices] IRequestHandler<LookupOrderByOrderNumberCommand, TraceableOrder> lookupOrderQuery,
            [FromServices] IRequestHandler<AllChangesCommand, Unit> aggOrderChangesHandler, 
            CancellationToken cancellationToken)
        {

            _logger.LogInformation("{msg} {data}", "ChangeCollecting entered", request.XmlSerialize());

            var fozzyOrder = request;

            if (!fozzyOrder.IsValid())
                throw new CollectException(CollectErrors.InvalidFozzyShopOrder, Consts.GeneralErrors.FozzyShopOrderIsInvalid);

            var fozzyOrderId = fozzyOrder.GetOrder().orderId.ToInt(); // external order id is INT

            var currentOrder = await lookupOrderQuery.Handle(
                new LookupOrderByOrderNumberCommand()
                {
                    ExternalOrderNumber = fozzyOrderId
                }, cancellationToken);

            if (currentOrder == null)
                return ConfirmResponse.NotFound();

            await aggOrderChangesHandler.Handle(new AllChangesCommand()
            {
                Request = 
                new CollectingChangeRequest() 
                { 
                    CurrentOrder = currentOrder,
                    ChangedOrder = fozzyOrder
                }
                
            }, cancellationToken);

            return ConfirmResponse.OK();
        }


        // TODO userInn!
        // moved to domain
        /*
        private TraceableOrder MapDtoToEntityWithoutPickerId(Guid id, Guid basketId, OrderData orderData)
        {
            var order = orderData.GetOrder();
            var lines = orderData.orderLines;

            var fozzyOrder = new TraceableOrder()
            {
                BasketId = basketId,
                ChequePrintDateTime = order.chequePrintDateTime.ToDateTime(),
                ClientFullName = order.clientFullName,
                ClientMobilePhone = order.clientMobilePhone,
                ClientMobilePhoneAlt1 = order.clientMobilePhoneAlt1,
                ContainerBarcodes = order.containerBarcodes,
                ContragentFullName = order.contragentFullName,
                ContragentOKPO = order.contragentOKPO,
                DateModified =  order.dateModified.ToDateTime() ?? 
                    throw new CollectException(CollectErrors.InvalidDateModified, order.dateModified),
                DeliveryAddress = order.deliveryAddress,
                DeliveryDate = order.deliveryDate.ToDateTime() ??
                    throw new CollectException(CollectErrors.InvalidDeliveryDate, order.deliveryDate),
                DeliveryId = order.deliveryId,
                DeliveryTimeFrom = order.deliveryTimeFrom.ToTime() ?? 
                    throw new CollectException(CollectErrors.InvalidDeliveryTimeFrom, order.deliveryTimeFrom),
                DeliveryTimeTo = order.deliveryTimeTo.ToTime() ??
                    throw new CollectException(CollectErrors.InvalidDeliveryTimeTo, order.deliveryTimeTo),
                DriverId = order.driverId.ToInt(),
                DriverName = order.driverName,
                ExternalOrderId = order.orderId.ToIntNull() ??
                    throw new CollectException(CollectErrors.InvalidOrderIdAsInt, order.orderId),
                FilialId = order.filialId,
                Id = id,
                LastContainerBarcode = order.lastContainerBarcode,  
                LogisticsType = order.logisticsType,
                MegaContainerBarcodes = order.megaContainerBarcodes,
                OrderBarcode = order.orderBarcode,  
                OrderCreated = order.orderCreated.ToDateTime() ??
                    throw new CollectException(CollectErrors.InvalidCreatedOrder, order.orderCreated),
                OrderNumber = order.orderId,
                OrderStatus = order.orderStatus.ToInt(),
                PaymentId = order.paymentId.ToInt(),
                PlacesCount = order.placesCount.ToInt(),
                Priority = order.priority,
                Remark = order.remark,  
                RroNumber  = order.rroNumber,
                SumPaymentFromInternet = order.sumPaymentFromInternet.ToDecimal(),
                SumPaymentFromKassa = order.sumPaymentFromKassa.ToDecimal(),

                OrderFrom = order.orderFrom.ToInt(),
                GlobalUserId = null, // TODO Set pickerId  
                UserInn = order.globalUserId // это ИНН сейчас
            };
            foreach(var line in lines)
            {
                var item = new TraceableOrderItem()
                {
                    BasketId = fozzyOrder.BasketId,
                    ContainerBarcode = line.containerBarcode,
                    CustomParams = line.CustomParams,
                    DateModified = line.dateModified.ToDateTime() ??
                        throw new CollectException(CollectErrors.InvalidDateModifiedLine, line.dateModified),
                    ExternalOrderId = fozzyOrder.ExternalOrderId,
                    FreezeStatus = line.freezeStatus,
                    Id = line.GetLineId() ?? Guid.NewGuid(),
                    GlobalUserId = null, //fozzyOrder.GlobalUserId, //line.globalUserId.ToIntNull(),
                    LagerId = line.lagerId,
                    IsActivityEnable = line.IsActivityEnable.ToBool(),
                    LagerName = line.lagerName,
                    LagerUnit = line.lagerUnit,
                    OrderQuantity = line.orderQuantity.ToDecimal(),
                    OrderNumber = fozzyOrder.OrderNumber,
                    PickerQuantity = line.pickerQuantity.ToDecimalNull(),
                    PriceOut = line.priceOut.ToDecimal(),
                    //ReplacementOnLagerId = line.re
                    ReplacementLagers = line.replacementLagers,
                    IsWeighted = line.GetIsWeighted() ?? new bool?(), // TODO
                    RowNum = line.rowNum,
                    UserInn  = line.globalUserId,
                    //PickerName = line.Pi
                };
                fozzyOrder.Items.Add(item); 
            }

            return fozzyOrder;
        }
        */




        [HttpGet]
        [Route(ExpansionConsts.Common.App.Controllers.Collecting.ThrowUnhandledExceptionRoute)]
        [ProducesResponseType(StatusCodes.Status501NotImplemented)]
        [CollectingExceptionHandler]
        public IActionResult ThrowUnhandled()
        {
            throw new Exception("Test unhandled exception");
        }

        [HttpGet]
        [Route(ExpansionConsts.Common.App.Controllers.Collecting.ThrowBusinessExceptionRoute)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [CollectingExceptionHandler]
        public IActionResult ThrowBusiness()
        {
            throw new CollectException(CollectErrors.BusinessExceptionTest, "Тестовая ошибка!");
        }


        [HttpGet]
        [Route(ExpansionConsts.Common.App.Controllers.Collecting.HealthCheckRoute)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Ping()
        {
            return Ok();
        }
    }
}