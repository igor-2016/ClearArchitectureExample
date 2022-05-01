using ApplicationServices.Implementation;
using AutoMapper;
using DomainServices.Implementation;
using ECom.Expansion.TestHelpers;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using WebSite.Presta.Clients;
using WebSite.Presta.Clients.Options;
using Xunit;

namespace ECom.Expansion.IntegrationTests.Clients.WebSite
{
    public class WebSitePrestaApiTests
    {

        
        private const string PutOrderMethod = "/putordertofozzyshop";

        private const string BaseUrl_Valid = "https://fozzyshop.in.ua";

        TestDataGenerator _gn;
        IMapper _mapper;
        public WebSitePrestaApiTests()
        {
            _gn = new TestDataGenerator();
            _mapper = new TestAutoMapper().GetMapper();
        }


        /// <summary>
        /// Бросает ордер на сайт
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Can_SaveNewOrder_To_FozzyWebSite()
        {
            var cancellationToken = CancellationToken.None;
            var loggerMock = new Mock<ILogger<FozzyShopSiteServiceClient>>();
            var options = new MockOptions<FozzyShopSiteOptions>(
                new FozzyShopSiteOptions()
                {
                    BaseUrl = BaseUrl_Valid,
                    PutOrderMethod = PutOrderMethod,
                    Login = "britoff",
                    Password = "123654",
                    Timeout = TimeSpan.FromMilliseconds(10000)
                });

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

            traceableOrder.OrderStatus = 913;

            var orderData = transformService.ToFozzyOrder(traceableOrder);

            var client = new FozzyShopSiteServiceClient(options, loggerMock.Object);
            var result = await client.PutOrderData(orderData, cancellationToken);

            Assert.True(result.IsSuccess);
            Assert.False(result.HasError);

            //OrderData readOrder;
            //var readResult = await client.GetOrderData(ordeNumber);
            //Assert.True(readResult.IsSuccess);
            //Assert.False(readResult.HasError);
            //readOrder = readResult.Order;
        }
    }
}
