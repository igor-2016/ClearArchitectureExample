using DataAccess.Interfaces;
using DataAccess.Interfaces.Dto;
using DataAccess.Interfaces.Specifications;
using DomainServices.Interfaces;
using ECom.Entities.Models.Workflow;
using Entities.Models.Expansion;
using Workflow.Interfaces;
using Workflow.Interfaces.Clients;
using Workflow.Interfaces.Requests;

namespace Workflow.Ecom
{
    public class EComWorkflowService : IWorkflowService
    {
        private readonly IDataAccess _dataAccess;
        //private readonly IReadOnlyDataAccess _readOnlyDataAccess;
        private readonly IWorkflowServiceClient _workflowClient;
        private readonly ITransformService _transformService;
        public EComWorkflowService(
            IDataAccess dataAccess, 
            //IReadOnlyDataAccess readOnlyDataAccess,
            IWorkflowServiceClient workflowClient,
            ITransformService transformService)
        {
            _workflowClient = workflowClient;
            _dataAccess = dataAccess;
            //_readOnlyDataAccess = readOnlyDataAccess;
            _transformService = transformService;
        }

        public async Task<TraceableOrder> CreateOrder(TraceableOrder newOrder, CancellationToken cancellationToken)
        {
            return await _dataAccess.AddNewAsync<TraceableOrder, TraceableOrderDto, Guid>(newOrder, cancellationToken);
        }

        public async Task<TraceableOrder> UpdateOrder(TraceableOrder order, CancellationToken cancellationToken)
        {
            var specification = new GetOrderByOrdeIdWithItems(order.Id);
            return await _dataAccess.UpdateAsync<TraceableOrder, TraceableOrderDto, Guid>(order, specification, cancellationToken);
        }

        public async Task<TraceableOrder> GetOrderByBasketId(Guid basketId, CancellationToken cancellationToken)
        {
            var specification = new GetOrderByBasketIdWithItems(basketId);
            return await _dataAccess
                .FindFirstOrDefaultAsync<TraceableOrder, TraceableOrderDto, Guid>(specification, cancellationToken);
        }

        public async Task StartCollecting(int oldOrderStatusId, TraceableOrder order, CancellationToken cancellationToken)
        {
            var ecomPicker = order.GetEComPicker();

            await _workflowClient.OnChangeState(order.BasketId, oldOrderStatusId, 
                order.OrderStatus, ecomPicker, cancellationToken);
        }

        public async Task UpdateCollecting(int oldOrderStatusId, TraceableOrder order, CancellationToken cancellationToken)
        {
            var collectingItems = _transformService.ToCollectingItems(order);
            await _workflowClient.OnChangeState(order.BasketId, oldOrderStatusId,
                order.OrderStatus, collectingItems, cancellationToken);
        }
     
        public async Task DoneCollecting(int oldOrderStatusId, TraceableOrder order, CancellationToken cancellationToken)
        {
            var collectingItems = _transformService.ToBasketCollectingItems(order);
            await _workflowClient.OnChangeState(order.BasketId, oldOrderStatusId,
                order.OrderStatus, collectingItems, cancellationToken);
        }

        public async Task<WorkflowOrderState> GetOrderCurrentState(Guid basketId, CancellationToken cancellationToken)
        {
           return await _workflowClient.GetOrderCurrentState(basketId, cancellationToken);
        }

        public async Task DefaultAction(int oldOrderStatusId, TraceableOrder order, CancellationToken cancellationToken)
        {
            await _workflowClient.OnChangeStateNoData(order.BasketId, oldOrderStatusId, order.OrderStatus,
                cancellationToken);
        }

        public async Task DefaultAction(Guid basketId, int oldOrderStatusId, int newOrderStatusId, CancellationToken cancellationToken)
        {
            await _workflowClient.OnChangeStateNoData(basketId, oldOrderStatusId, newOrderStatusId, cancellationToken);
        }
    }
}