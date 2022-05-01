using AutoMapper;
using Collecting.Interfaces;
using Collecting.Interfaces.Clients;
using Collecting.Interfaces.Clients.Responses;
using DataAccess.Interfaces;
using DataAccess.Interfaces.Dto;
using DataAccess.Interfaces.Specifications;
using DomainServices.Interfaces;
using Entities.Models.Collecting;
using Entities.Models.Expansion;
using WebSiteService.Interfaces.Clients;

namespace Collecting.Fozzy
{
    public class FozzyCollectingService : ICollectingService
    {
        private readonly IDataAccess _dataAccess;
        private readonly ITransformService _transformService;
        private readonly IFozzyCollectingServiceClient _fozzyCollectingServiceClient;
        private readonly IFozzyStaffServiceClient _fozzyStaffClient;
        private readonly IFozzyShopSiteServiceClient _fozzyShopSiteServiceClient;
        private readonly IMapper _mapper;

        public FozzyCollectingService(
            IDataAccess dataAccess, 
            ITransformService transformService,
            IFozzyCollectingServiceClient fozzyShopCollectingServiceClient,
            IFozzyStaffServiceClient  fozzyShopStaffClient,
            IFozzyShopSiteServiceClient fozzyShopSiteServiceClient,
            IMapper mapper
            )
        {
            _mapper = mapper;
            _transformService = transformService;
            _dataAccess = dataAccess;
            _fozzyCollectingServiceClient = fozzyShopCollectingServiceClient;
            _fozzyStaffClient = fozzyShopStaffClient;
            _fozzyShopSiteServiceClient = fozzyShopSiteServiceClient;
        }

        

        public async Task<TraceableOrder> GetOrderByBasketId(Guid basketId, CancellationToken cancellationToken)
        {
            var specification = new GetOrderByBasketIdWithItems(basketId);
            return await _dataAccess
                .FindFirstOrDefaultAsync<TraceableOrder, TraceableOrderDto, Guid>(specification, cancellationToken);
        }

        public async Task<TraceableOrder> GetOrderByExternalOrderId(int externalOrderId, CancellationToken  cancellationToken)
        {
            var specification = new GetOrderByExternalOrderIdWithItems(externalOrderId);
            return await _dataAccess
                .FindFirstOrDefaultAsync<TraceableOrder, TraceableOrderDto, Guid>(specification, cancellationToken);
        }
        public async Task<TraceableOrder> GetOrderByOrderNumber(string orderNumber, CancellationToken cancellationToken)
        {
            var specification = new GetOrderByOrderNumberWithItems(orderNumber);
            return await _dataAccess
                .FindFirstOrDefaultAsync<TraceableOrder, TraceableOrderDto, Guid>(specification, cancellationToken);
        }


        public async Task<TraceableOrder> GetOrderByOrderId(string orderId, CancellationToken cancellationToken)
        {
            var specification = new GetOrderByOrderNumberWithItems(orderId);
            return await _dataAccess
                .FindFirstOrDefaultAsync<TraceableOrder, TraceableOrderDto, Guid>(specification, cancellationToken);
        }

        public async Task<OrderData> SendOrderToFozzyWebService(TraceableOrder order, CancellationToken cancellationToken)
        {
            var orderToSend = _transformService.ToFozzyOrder(order);
            var fozzyOrderData = _mapper.Map<FozzyOrderData>(orderToSend);
            var result = await _fozzyCollectingServiceClient.PutOrderData(fozzyOrderData, cancellationToken);
            if (!result.IsSuccess)
                throw result.AnError;
            return orderToSend;
        }

        public async Task<OrderData> SendOrderToFozzyWebSite(TraceableOrder order, CancellationToken cancellationToken)
        {
            var orderToSend = _transformService.ToFozzyOrder(order);
            var result = await _fozzyShopSiteServiceClient.PutOrderData(orderToSend, cancellationToken);
            if (!result.IsSuccess)
                throw result.AnError;
            return orderToSend;
        }


        public async Task<TraceableOrder> UpdateOrder(TraceableOrder order, CancellationToken cancellationToken)
        {
            var specification = new GetOrderByOrdeIdWithItems(order.Id);
            return await  _dataAccess.UpdateAsync<TraceableOrder, TraceableOrderDto, Guid>(order, specification, cancellationToken);
        }


        public async Task<Picker> GetFozzyPickerById(int globalUserId, CancellationToken cancellationToken)
        {
           var result =  await _fozzyStaffClient.GetStaffInfoByGlobalUserId(globalUserId, cancellationToken);

            if (result.IsSuccess)
                return result.Staff.ToPicker();

            throw result.AnError;
        }

        public async Task<Picker> GetFozzyPickerByInn(string inn, CancellationToken cancellationToken)
        {
            var result = await _fozzyStaffClient.GetStaffInfoByInn(inn, cancellationToken);

            if (result.IsSuccess)
                return result.Staff.ToPicker();

            throw result.AnError;
        }


        public async Task SaveFozzyOrder(OrderData orderData, CancellationToken cancellationToken)
        {
            var fozzyOrder = _mapper.Map<FozzyOrderData>(orderData);
            var result = await _fozzyCollectingServiceClient.PutOrderData(fozzyOrder, cancellationToken);

            if (!result.IsSuccess)
                 throw result.AnError;

        }

        public async Task<OrderData> GetFozzyOrder(string orderId, CancellationToken cancellationToken)
        {
            var result = await _fozzyCollectingServiceClient.GetOrderData(orderId, cancellationToken);

            if (result.IsSuccess)
                return _mapper.Map<OrderData>(result.Order);

            throw result.AnError;
        }

    }
}