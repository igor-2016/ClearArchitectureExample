using ApplicationServices.Interfaces.Enums;
using Catalog.Interfaces;
using Collecting.Interfaces.Enums;
using DomainServices.Interfaces;
using ECom.Entities.Models;
using ECom.Types.Delivery;
using ECom.Types.Orders;
using Entities.Models.Expansion;
using MediatR;
using Microsoft.Extensions.Logging;
using Utils.Attributes;
using Workflow.Interfaces;
using Workflow.Interfaces.Exceptions;
using Workflow.Interfaces.Requests;
using Workflow.UseCases.Consts;

namespace Workflow.UseCases.Order
{
    [ApmTrace]
    public class AcceptReplacementsCommand : IRequest<TraceableOrder>
    {
        public ICollectingAcceptAfterApproveRequest Request { get; set; }
    }

    /// <summary>
    /// Вызывается после уточнения замен 
    /// </summary>
    public class AcceptReplacementsCommandHandler : WorkflowCommandHandlerBase,
        IRequestHandler<AcceptReplacementsCommand, TraceableOrder>
    {
        private readonly IWorkflowService _workflowService;
        private readonly ITransformService _transformService;
        private readonly ICatalogService _catalogService;

        public AcceptReplacementsCommandHandler(
            ILogger<AcceptReplacementsCommandHandler> logger,
            ICatalogService catalogService,
            ITransformService transformService,
            IWorkflowService workflowService,
            IMediator mediator) : base (mediator, logger)
        {
            _workflowService = workflowService;
            _transformService = transformService;
            _catalogService = catalogService;
        }

        public async Task<TraceableOrder> Handle(AcceptReplacementsCommand request,
            CancellationToken cancellationToken)
        {
            var data = request.Request;
            var ecomCollectableOrderInfo = request.Request.AcceptedOrder;

            _logger.LogInformation("{msg} {data}", WorkflowConsts.CommandInfo.AcceptReplacementsCommandName, data);

            var currentOrder = await _workflowService.GetOrderByBasketId(data.BasketGuid, cancellationToken);

            if (currentOrder == null)
                throw new WorkflowException(WorkflowErrors.OrderNotFound);

            // скасоване
            await _mediator.Send(new CheckRefusedOrderCommand()
            {
                Order = currentOrder,
            });

            if (currentOrder.OrderStatus != (int)FozzyOrderStatus.Status914)
                throw new WorkflowException(WorkflowErrors.InvalidOrderReplacementStatus);


            var acceptedOrder = await _transformService.FromAcceptedOrder(
                ecomCollectableOrderInfo,
                currentOrder,
                            GetCatalogSingleItemInfo,
                        DefaultRowNumCalculator,
                        NoCheckFilledCalculator,
                        _transformService.GetLogisticType,
                        cancellationToken
                        );
                
            currentOrder = await _mediator.Send(new UpdateQtyReplacementsFromEcomCommand()
            {
                OrderToBeUpdated = currentOrder, // view

                SourceOfChanges = acceptedOrder // order data
            });



            currentOrder.CollectingState = (int)OrderCollectingState.UpCollecting;
            currentOrder.OrderStatus = (int)FozzyOrderStatus.Status915;

            // сохранить изменения
            currentOrder = await _workflowService.UpdateOrder(currentOrder, cancellationToken);

            return currentOrder;
        }


        #region process delegates

        private async Task<CatalogInfo> GetCatalogSingleItemInfo(int lagerId, int filialId, Merchant merchant, 
            CancellationToken cancellationToken,
            DeliveryType ecomDeliveryType = DeliveryType.Unknown, Guid? basketGuid = null)
        {
            return await _catalogService.GetCatalogItems(lagerId, filialId, (int)merchant, cancellationToken,
                (int)ecomDeliveryType, basketGuid);
        }



        //TODO
        private void NoCheckFilledCalculator(IEnumerable<TraceableOrderItem> items)
        {

        }


        //TODO
        private void DefaultRowNumCalculator(IEnumerable<TraceableOrderItem> items)
        {
            var rowId = 1;
            foreach (var item in items)
            {
                item.RowNum = rowId;
                rowId++;
            }
        }

      

        #endregion process delegates

    }
}
