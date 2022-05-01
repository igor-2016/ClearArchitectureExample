using ApplicationServices.Interfaces.Enums;
using Catalog.Interfaces;
using Collecting.Interfaces.Enums;
using DomainServices.Interfaces;
using ECom.Entities.Models;
using ECom.Types.Collect;
using ECom.Types.Delivery;
using ECom.Types.Exceptions;
using ECom.Types.Orders;
using Entities.Models.Expansion;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Utils;
using Utils.Attributes;
using Workflow.Interfaces;
using Workflow.Interfaces.Exceptions;
using Workflow.UseCases.Consts;

namespace Workflow.UseCases.Order
{

    [ApmTrace]
    public class CreateOrderCommand : IRequest<TraceableOrder>
    {
        public FozzyCollectableOrderInfo EComCollectableOrder { get; set; }
    }

    public class CreateOrderComandHandler :
        WorkflowCommandHandlerBase,
        IRequestHandler<CreateOrderCommand, TraceableOrder>
    {
        private readonly IWorkflowService _workflowService;
        private readonly ITransformService _transformService;
        private readonly ICatalogService _catalogService;

        public CreateOrderComandHandler(
            ILogger<CreateOrderComandHandler> logger,
            IWorkflowService workflowService,
            ITransformService  transformService, 
            ICatalogService catalogService,
            IMediator mediator) : base (mediator, logger)
        {
            _workflowService = workflowService;
            _transformService = transformService;
            _catalogService = catalogService;
        }


        public async Task<TraceableOrder> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var ecomCollectableOrderInfo = request.EComCollectableOrder;

            using (_logger.LoggingScope(ecomCollectableOrderInfo.BasketGuid))
            {
                _logger.LogInformation("{msg} {data}", WorkflowConsts.CommandInfo.CreateCollectingCommandName, ecomCollectableOrderInfo);

                Stopwatch sw = Stopwatch.StartNew();
                try
                {
                    var existingOrder = await _workflowService.GetOrderByBasketId(ecomCollectableOrderInfo.BasketGuid, cancellationToken);

                    if (existingOrder != null)
                    {
                        if (existingOrder.CollectingState == (int)OrderCollectingState.New)
                        {
                            _logger.LogWarning("Trying to create fozzy shop order that exists and  in (\"New\") state.");
                            return existingOrder;
                        }

                        throw new WorkflowException(WorkflowErrors.OrderIsAlreadyInCollecting, $"текущий статус: {existingOrder.OrderStatus}");
                    }

                    _logger.LogInformation("{msg} {data}", "ecomCollectableOrderInfo", ecomCollectableOrderInfo.JsonSerialize());

                    var newTraceableOrder = await _transformService.ToNewTraceableOrder(
                        ecomCollectableOrderInfo,
                            GetCatalogSingleItemInfo,
                              Get_Site_OrderOrigin,
                              RowNumCalculator,
                              _transformService.GetLogisticType,
                              cancellationToken
                        );

                    _logger.LogInformation("{msg} {data}", "newTraceableOrder", newTraceableOrder.JsonSerialize());
                    newTraceableOrder.OrderStatus =  newTraceableOrder.OrderStatus == 0 
                        ? (int)FozzyOrderStatus.Status913 : newTraceableOrder.OrderStatus;
                    newTraceableOrder.CollectingState = (int)OrderCollectingState.New;

                    newTraceableOrder = await _workflowService.CreateOrder(newTraceableOrder, cancellationToken);
                    _logger.LogInformation("{msg} {data}", "savedTraceableOrder", newTraceableOrder.JsonSerialize());

                    return newTraceableOrder;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "{msg} {error} {StackTrace}", "Error occurred while CreateOrder", ex.FullMessage(), ex.StackTrace);

                    throw; 
                }
            }
        }


        #region process delegates

        private async Task<CatalogInfo> GetCatalogSingleItemInfo(int lagerId, int filialId, Merchant merchant, 
            CancellationToken cancellationToken,
            DeliveryType ecomDeliveryType = DeliveryType.Unknown, Guid? basketGuid = null)
        {
            return await _catalogService.GetCatalogItems(lagerId, filialId, (int)merchant, cancellationToken, 
                (int)ecomDeliveryType, basketGuid);
        }


       

        private int? Get_Site_OrderOrigin(int merchantId, int filialId, BasketType basketType)
        {
            return (int)FozzyOrderOrigin.Site;
        }
        
        private void RowNumCalculator(IEnumerable<TraceableOrderItem> items)
        {
            var rowId = 1;
            foreach(var item in items)
            {
                item.RowNum = rowId;
                rowId++;
            }
        }


        #endregion process delegates

    }
}
