using ApplicationServices.Implementation;
using AutoMapper;
using Collecting.Fozzy.Clients;
using Collecting.Fozzy.Clients.Options;
using Collecting.Interfaces.Clients;
using Collecting.Interfaces.Clients.Responses;
using DataAccess.Interfaces;
using DataAccess.Interfaces.Dto;
using DataAccess.MsSql;
using DomainServices.Implementation;
using ECom.Expansion.TestHelpers;
using Entities.Models.Collecting;
using Entities.Models.Expansion;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ECom.Expansion.IntegrationTests.Clients.Collect
{
    public class CollectingFozzyApiTests
    {

        private const string GetByOrderMethodFormat = "/FozzyShopOrderService.svc/GetOrderData?orderId={0}";

        private const string PutOrderMethod = "/FozzyShopOrderService.svc/PutOrderData";

        private const string BaseUrl_Valid = "https://s-kv-center-x57.officekiev.fozzy.lan:1449/";

        readonly DbContextOptions<AppDbContext> _sqlServerOptions;
        readonly IDataAccess _expansionSqlServerDataAccess;

        TestDataGenerator _gn;
        IMapper _mapper;
        public CollectingFozzyApiTests()
        {
            _gn = new TestDataGenerator();
            _mapper = new TestAutoMapper().GetMapper();

            _sqlServerOptions = new DbContextOptionsBuilder<AppDbContext>()
                    .UseSqlServer("Server=.;Database=EComExpansionDev;Trusted_Connection=True")
                    .Options;

            _expansionSqlServerDataAccess = new AppDbContext(_sqlServerOptions, _mapper);
        }

       [Fact]
       public async Task Can_SaveNewOrder_To_FozzyWebService()
        {
            CancellationToken cancellationToken = CancellationToken.None;
            var loggerFozzyShopClientMock = new Mock<ILogger<IFozzyCollectingServiceClient>>();
            var options = new MockOptions<FozzyShopCollectingServiceOptions>(new FozzyShopCollectingServiceOptions() 
                { BaseUrl = BaseUrl_Valid, GetByOrderMethodFormat = GetByOrderMethodFormat, PutOrderMethod= PutOrderMethod });

            var basketId = Guid.NewGuid();
            var line1Id = Guid.NewGuid();
            var line2Id = Guid.NewGuid();
            var ordeNumber = "12434595";

            var orderInfo = _gn.CreateEComBasketWithTwoLines(basketId, line1Id, line2Id, orderId: ordeNumber);

            var mappingService = new EntityMapper();
            var dateTimeNow = DateTime.Now;
            var dateService = new DateTimeService(dateTimeNow);

            var transformService = new TransformService(_mapper, mappingService, dateService);

            var traceableOrder = await transformService.ToNewTraceableOrder(
              orderInfo,
              _gn.GetValidCatalogSingleItemInfo,
              _gn.Get_Site_OrderOrigin,
              _gn.DefaultRowNumCalculator,
              transformService.GetLogisticType,
              cancellationToken
              );

            traceableOrder.OrderStatus = 913; // Новый

            var orderData = transformService.ToFozzyOrder(traceableOrder);
            var fozzyOrderData = _mapper.Map<FozzyOrderData>(orderData);

            // save to db
            var dataAccess = _expansionSqlServerDataAccess;
            var createdTraceableOrder = await dataAccess.AddNewAsync<TraceableOrder, TraceableOrderDto, Guid>(traceableOrder);
            Assert.NotNull(createdTraceableOrder);
            Assert.Equal(traceableOrder.Id, createdTraceableOrder.Id);

            OrderData readOrder; 

            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(options.Value.BaseUrl);
                var fozzyWebServiceClient = new FozzyCollectingServiceClient(httpClient, options, loggerFozzyShopClientMock.Object);
                var result = await fozzyWebServiceClient.PutOrderData(fozzyOrderData, cancellationToken);

                Assert.True(result.IsSuccess);
                Assert.False(result.HasError);


                var readResult = await fozzyWebServiceClient.GetOrderData(ordeNumber, cancellationToken);
                Assert.True(readResult.IsSuccess);
                Assert.False(readResult.HasError);
                Assert.NotNull(readResult.Order);
                

                readOrder = _mapper.Map<OrderData>(readResult.Order);

            }

            var currentPicker = traceableOrder.GetPicker();

            // and conver to traceable
            var fromFozzyTraceableOrder = await transformService.FromFozzyOrder(
                traceableOrder,
                currentPicker,
                readOrder,
                _gn.GetValidCatalogSingleItemInfo,
                PickerInfoExtractorByInn,
                NoCheckFilledCalculator,
                DefaultRowNumCalculator,
                transformService.GetLogisticType,
                cancellationToken);

            Assert.NotNull(fromFozzyTraceableOrder);

            fromFozzyTraceableOrder.Id = Guid.NewGuid();
            fromFozzyTraceableOrder.RowVersion = "";
            fromFozzyTraceableOrder.BasketId = Guid.NewGuid();
            foreach (var item in fromFozzyTraceableOrder.Items)
            {
                item.Id = Guid.NewGuid();
                item.OrderId = fromFozzyTraceableOrder.Id;
                item.BasketId = fromFozzyTraceableOrder.BasketId;
            }

            var createdTraceableOrder2 = await dataAccess
                .AddNewAsync<TraceableOrder, TraceableOrderDto, Guid>(fromFozzyTraceableOrder);

            Assert.NotNull(createdTraceableOrder2);
            //Assert.Equal(traceableOrder.Id, createdTraceableOrder2.Id);
            Debug.WriteLine($"('{traceableOrder.Id}', '{createdTraceableOrder2.Id}')");
            //TODO compare all
        }

        private Task<Picker> PickerInfoExtractorByInn(string inn, CancellationToken cancellationToken)
        {
            return Task.FromResult( _gn.GetValidPicker());
        }

        private void NoCheckFilledCalculator(IEnumerable<TraceableOrderItem> items)
        {

        }
     
        private void DefaultRowNumCalculator(IEnumerable<TraceableOrderItem> items)
        {
            var rowId = 1;
            foreach (var item in items)
            {
                item.RowNum = rowId;
                rowId++;
            }
        }
    }
}
