using ApplicationServices.Implementation.ChangeTracker;
using ApplicationServices.Implementation.ChangeTracker.OrderItems.Handlers;
using ApplicationServices.Implementation.EventHandlers;
using ApplicationServices.Interfaces;
using ApplicationServices.Interfaces.Models;
using ApplicationServices.Interfaces.Requests;
using AutoMapper;
using Catalog.Interfaces;
using DataAccess.Interfaces;
using DataAccess.Interfaces.Dto;
using DataAccess.Interfaces.Specifications;
using DomainServices.Interfaces;
using Entities.Models.Expansion;
using Microsoft.Extensions.Logging;
using Workflow.Interfaces;

namespace ApplicationServices.Implementation
{
    public class CommonAppService : ICommonAppService
    {
        private readonly IWorkflowService _workflowService;
        private readonly ITransformService _transformService;
        private readonly ICatalogService _catalogService;
        private readonly ILogger<CommonAppService> _logger;
        private readonly IDataAccess _dataAccess;
      //  private readonly IReadOnlyDataAccess _readOnlyDataAccess;
        private readonly IMapper _mapper;

        public CommonAppService(
            IWorkflowService workflowService,
            ICatalogService catalogService,
            ITransformService transformService,
            IDataAccess dataAccess,
           // IReadOnlyDataAccess readOnlyDataAccess,
            IMapper mapper,
            ILogger<CommonAppService> logger
            )
        {
            _workflowService = workflowService;
            _transformService = transformService;
            _catalogService = catalogService;
            _logger = logger;
            _dataAccess = dataAccess;
          //  _readOnlyDataAccess = readOnlyDataAccess;
            _mapper = mapper;
        }

        
        public async Task StartCollecting(int oldOrderStatusId, TraceableOrder order,
            CancellationToken cancellationToken)
        {
            await _workflowService.StartCollecting(oldOrderStatusId, order, cancellationToken);
        }
        
        public async Task UpdateCollecting(int oldOrderStatusId, TraceableOrder order,
            CancellationToken cancellationToken)
        {
            await _workflowService.UpdateCollecting(oldOrderStatusId, order, cancellationToken);
        }

        public async Task DoneCollecting(int oldOrderStatusId, TraceableOrder order,
            CancellationToken cancellationToken)
        {
            await _workflowService.DoneCollecting(oldOrderStatusId, order, cancellationToken);
        }

        public async Task DefaultAction(int oldOrderStatusId, TraceableOrder order,
            CancellationToken cancellationToken)
        {
            await _workflowService.DefaultAction(oldOrderStatusId, order, cancellationToken);
        }

        public async Task NotifyWorkflowStatusChanged(Guid basketId, int oldOrderStatusId, int newOrderStatusId,
            CancellationToken cancellationToken)
        {
            await _workflowService.DefaultAction(basketId, oldOrderStatusId, newOrderStatusId, cancellationToken);
        }

        public async Task<TraceableOrder> ResearchOrder(ICollectableOrdersInput input,
            CancellationToken cancellationToken)
        {
            var orderToBeUpdated = input.OrderToBeUpdated;
            var sourceOfChanges = input.SourceOfChanges;
            // change picket quantity
            var logAllPropertiesChangesHandler =
                new LogAllPropertiesChangesHandler(nameof(LogAllPropertiesChangesHandler), _logger);
            //..

            // property order item tracker
            var propertiesChangeTracker = new OrderItemPropsChangeTracker();
            logAllPropertiesChangesHandler.Subscribe(propertiesChangeTracker);

            //------------------------
            // обновление
            var updateOrderItemHandler = new UpdateOrderItemHandler(nameof(UpdateOrderItemHandler),
                _logger, propertiesChangeTracker);

            // order items tracker
            var itemsChangeTracker = new OrderItemsChangeTracker();
            updateOrderItemHandler.Subscribe(itemsChangeTracker);

            // поиск изменений
            await itemsChangeTracker.TrackChanges(sourceOfChanges.Items, orderToBeUpdated.Items, cancellationToken)
                        .ContinueWith(c => itemsChangeTracker.Done(cancellationToken), cancellationToken);

            return orderToBeUpdated;
        }


        // done
        public async Task<TraceableOrder> AcceptCollecting(ICollectableOrdersInput input,
            CancellationToken cancellationToken)
        {
            var orderToBeUpdated = input.OrderToBeUpdated;
            var sourceOfChanges = input.SourceOfChanges;

            // change picket quantity
            var changePickedQtyHandler =
                new ChangePickedQtyHandler(nameof(ChangePickedQtyHandler), _logger, sourceOfChanges);
            //..

            // property order item tracker
            var propertiesChangeTracker = new OrderItemPropsChangeTracker();
            changePickedQtyHandler.Subscribe(propertiesChangeTracker);

            //------------------------
            // обновление
            var updateOrderItemHandler = new UpdateOrderItemHandler(nameof(UpdateOrderItemHandler),
                _logger, propertiesChangeTracker);

            // order items tracker
            var itemsChangeTracker = new OrderItemsChangeTracker();
            updateOrderItemHandler.Subscribe(itemsChangeTracker);

            // поиск изменений
            await itemsChangeTracker.TrackChanges(sourceOfChanges.Items, orderToBeUpdated.Items, cancellationToken)
                        .ContinueWith(c => itemsChangeTracker.Done(cancellationToken), cancellationToken);

            return orderToBeUpdated;
        }

        public async Task<TraceableOrder> OfferReplacements(ICollectableOrdersInput input,
            CancellationToken cancellationToken)
        {
            var orderToBeUpdated = input.OrderToBeUpdated;
            var sourceOfChanges = input.SourceOfChanges;

            // change picket quantity
            var changePickedQtyHandler = 
                new ChangePickedQtyHandler(nameof(ChangePickedQtyHandler), _logger, sourceOfChanges);
            // change replacements lager
            var changeReplacementsHandler = new ChangeReplacementsHandler(nameof(ChangeReplacementsHandler),
               orderToBeUpdated, sourceOfChanges, _logger, _catalogService, _transformService);
            //..

            // property order item tracker
            var propertiesChangeTracker = new OrderItemPropsChangeTracker();
            changeReplacementsHandler.Subscribe(propertiesChangeTracker);
            changePickedQtyHandler.Subscribe(propertiesChangeTracker);

            //------------------------
            //добавление новой позиции
            var addNewOrderItemHandler = new AddNewOrderItemHandler(nameof(AddNewOrderItemHandler),
              orderToBeUpdated, _catalogService, _transformService, true, _logger);
            // обновление
            var updateOrderItemHandler = new UpdateOrderItemHandler(nameof(UpdateOrderItemHandler), 
                _logger, propertiesChangeTracker);
            // удаление позиции
            var itemToDeleteHandler = new RemoveCurrentOrderItemHandler(nameof(RemoveCurrentOrderItemHandler),
                orderToBeUpdated, _logger);

            // order items tracker
            var itemsChangeTracker = new OrderItemsChangeTracker();
            addNewOrderItemHandler.Subscribe(itemsChangeTracker);
            updateOrderItemHandler.Subscribe(itemsChangeTracker);
            itemToDeleteHandler.Subscribe(itemsChangeTracker);

            // поиск изменений
            await itemsChangeTracker.TrackChanges(sourceOfChanges.Items, orderToBeUpdated.Items, cancellationToken)
                        .ContinueWith(c => itemsChangeTracker.Done(cancellationToken), cancellationToken);

            
            return orderToBeUpdated; 
        }

        public async Task<TraceableOrder> AcceptReplacements(ICollectableOrdersInput input,
            CancellationToken cancellationToken)
        {
            var orderToBeUpdated = input.OrderToBeUpdated;
            var sourceOfChanges = input.SourceOfChanges;


            //обновление qty в существующих позициях
            var сhangeOrderQuantityHandler = new ChangeOrderQuantityHandler(nameof(ChangeOrderQuantityHandler), _logger);
            var propertiesChangeTracker = new OrderItemPropsChangeTracker();
            сhangeOrderQuantityHandler.Subscribe(propertiesChangeTracker);
            //...

            //добавление новой позиции
            var addNewOrderItemHandler = new AddNewOrderItemHandler(nameof(AddNewOrderItemHandler),
              orderToBeUpdated, _catalogService, _transformService, true, _logger);
            var updateOrderItemHandler = new UpdateOrderItemHandler(nameof(UpdateOrderItemHandler), _logger, propertiesChangeTracker);

            var itemsChangeTracker = new OrderItemsChangeTracker();
            addNewOrderItemHandler.Subscribe(itemsChangeTracker);
            updateOrderItemHandler.Subscribe(itemsChangeTracker);

            // поиск изменений
            await itemsChangeTracker.TrackChanges(sourceOfChanges.Items, orderToBeUpdated.Items, cancellationToken)
                        .ContinueWith(c => itemsChangeTracker.Done(cancellationToken), cancellationToken);

            return orderToBeUpdated;
        }

        public async Task<TraceableOrder> GetOrderOnlyByBasketId(Guid basketId, CancellationToken cancellationToken)
        {
            var specification = new GetOrderByBasketIdNoItems(basketId);
            return await _dataAccess
                .FindFirstOrDefaultAsync<TraceableOrder, TraceableOrderDto, Guid>(specification, cancellationToken);
        }

        public async Task<TraceableOrder> GetOrderWithItemsByBasketId(Guid basketId, CancellationToken cancellationToken)
        {
            var specification = new GetOrderByBasketIdWithItems(basketId);
            return await _dataAccess
                .FindFirstOrDefaultAsync<TraceableOrder, TraceableOrderDto, Guid>(specification, cancellationToken);
        }

        public async Task<TraceableOrder> UpdateOrderOnly(TraceableOrder order, CancellationToken cancellationToken)
        {
            var specification = new GetOrderByOrdeIdNoItems(order.Id);
            return await _dataAccess.UpdateAsync<TraceableOrder, TraceableOrderDto, Guid>(order, specification, cancellationToken);
        }

        public async Task<TraceableOrder> UpdateOrderAndItems(TraceableOrder order, CancellationToken cancellationToken)
        {
            var specification = new GetOrderByOrdeIdWithItems(order.Id);
            return await _dataAccess.UpdateAsync<TraceableOrder, TraceableOrderDto, Guid>(order, specification, cancellationToken);
        }

        public async Task<TraceableOrder> CreateOrder(TraceableOrder newOrder, CancellationToken cancellationToken)
        {
            return await _dataAccess.AddNewAsync<TraceableOrder, TraceableOrderDto, Guid>(newOrder, cancellationToken);
        }

        public TraceableOrderView ToView(TraceableOrder order)
        {
            return _mapper.Map<TraceableOrderView>(order);
        }
    }
}