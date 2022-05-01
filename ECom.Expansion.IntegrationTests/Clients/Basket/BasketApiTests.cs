using ApplicationServices.Implementation;
using AutoMapper;
using Basket.Ecom;
using Basket.Ecom.Clients;
using Basket.Interfaces.Clients.Options;
using Catalog.Ecom;
using Catalog.Ecom.Clients;
using Catalog.Ecom.Options;
using Catalog.Interfaces;
using Collecting.Fozzy;
using Collecting.Fozzy.Clients;
using Collecting.Fozzy.Clients.Options;
using Collecting.Interfaces.Clients;
using DataAccess.Interfaces;
using DataAccess.MsSql;
using DomainServices.Interfaces;
using ECom.Entities.Models;
using ECom.Expansion.IntegrationTests.DataAccess.MsSql;
using ECom.Expansion.IntegrationTests.Services;
using ECom.Expansion.TestHelpers;
using ECom.Types.Delivery;
using ECom.Types.DTO;
using ECom.Types.Orders;
using Entities.Models.Collecting;
using Expansion.Ecom;
using Expansion.Ecom.Clients;
using Expansion.Ecom.Clients.Options;
using Expansion.Interfaces.Clients;
using Expansion.Interfaces.Dto;
using Expansion.Interfaces.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using WebSiteService.Interfaces.Clients;
using Workflow.Controllers;
using Workflow.UseCases.Order;
using Xunit;

namespace ECom.Expansion.IntegrationTests.Clients.Basket
{
    public class BasketApiTests
    {

        public const string GetBasketMethodFormat = "/ecom/{0}";
        public const string CreateBasketMethod = "/ecom/create";
        public const string ChangeOwnerMethodFormat = "/ecom/{0}/changeOwner";
        public const string ChangeFilialMethodFormat = "/ecom/{0}/changeFilialDelivery";
        public const string AddItemMethodFormat = "/ecom/{0}/addItem";
        public const string CloseBasketMethodFormat = "/ecom/{0}/propsClose";
        public const string GetTimeSlotMethodFormat = "/ecom/timeSlots/first?{0}";
        public const string GetCollactableOrderMethodFormat = "/ecom/workflow/order/{0}/getCollactableOrderInfo";
        public const string UpdateCollectingMethodFormat = "/ecom/workflow/order/{0}/updateCollectedItems";

        public const string BaseUrl_Valid = "https://ecom.test.fz.lan";
        public static TimeSpan Timeout = TimeSpan.Parse("00:10:30");

        readonly IDataAccess _testExpansionSqlServerDataAccess;
        readonly DbContextOptions<AppDbContext> _testSqlServerOptions;

        readonly IDataAccess _devExpansionSqlServerDataAccess;
        readonly DbContextOptions<AppDbContext> _devSqlServerOptions;

        readonly TestDataGenerator _gn;
        readonly IMapper _mapper;
        public BasketApiTests()
        {
            bool useTestDb = true;

            _gn = new TestDataGenerator();
            _mapper = MapperTest.GetMapper();
            _testSqlServerOptions = DbContextOptionsTest.GetDbContextOptions(useTestDb);
            _testExpansionSqlServerDataAccess = DbContextOptionsTest.GetSqlServerOptionsContext(useTestDb);

            _devSqlServerOptions = DbContextOptionsTest.GetDbContextOptions(!useTestDb);
            _devExpansionSqlServerDataAccess = DbContextOptionsTest.GetSqlServerOptionsContext(!useTestDb);
        }

        



        /// <summary>
        /// Создать ордер в ECom и в зависимости от маски CreateOrderAndCollectUseCases проходит по шагам
        /// При запуске этого теста нужно локальный или тестовый сервис экспансии (useLocalExpansionService)
        /// </summary>
        /// <returns></returns>
        [Theory]
        /*[InlineData(
            Filials.Filial_51,
            Merchant.Fozzy,
            RequestOrigin.Site,
            "0296018319764",
            BasketType.ClickAndCollect,
            "+380501111111", "",
            DeliveryAddressType.Flat,
            DeliveryType.DeliveryHome,
            "50.2", "60.4", true, "Неточные координаты", "Заказ с сайта на квартиру, домой",
            ItemsCombinations.TwoItems,
            "MjMxNDQ5ZDc0NTlkYjljOTY5NTE5YjBlOGYyMGU2MzE", "https://my.silpo.iir.fozzy.lan/account/purchases/my-orders",
            "contragent Full Name", "123456789",
            "i.ivanov@temabit.com", "Пётр Петрович Петров", "+380971111111",
            "+380972222222",
            OrderPaymentType.CourierCash,
            "постучать в дверь три раза",
            Business.Fozzy,
            CreateOrderAndCollectUseCases.CreateOrderAndCollect
            )]*/
        [InlineData(
            Filials.Filial_51,
            Merchant.Fozzy,
            RequestOrigin.Site,
            "0296018319764",
            BasketType.ClickAndCollect,
            "+380501111111", "",
            DeliveryAddressType.Flat,
            DeliveryType.DeliveryHome,
            "50.2", "60.4", true, "Неточные координаты", "Заказ с сайта на квартиру, домой",
            ItemsCombinations.TwoItems,
            "MjMxNDQ5ZDc0NTlkYjljOTY5NTE5YjBlOGYyMGU2MzE", "https://my.silpo.iir.fozzy.lan/account/purchases/my-orders",
            "contragent Full Name", "123456789",
            "i.ivanov@temabit.com", "Пётр Петрович Петров", "+380971111111",
            "+380972222222",
            OrderPaymentType.CourierCash,
            "постучать в дверь три раза",
            Business.Fozzy,
            CreateOrderAndCollectUseCases.CreateOrderCollectWithReplacements
            )]/*
        [InlineData(
            Filials.Filial_51,
            Merchant.Fozzy,
            RequestOrigin.Site,
            "0296018319764",
            BasketType.ClickAndCollect,
            "+380501111111", "",
            DeliveryAddressType.Flat,
            DeliveryType.DeliveryHome,
            "50.2", "60.4", true, "Неточные координаты", "Заказ с сайта на квартиру, домой",
            ItemsCombinations.TwoItems,
            "MjMxNDQ5ZDc0NTlkYjljOTY5NTE5YjBlOGYyMGU2MzE", "https://my.silpo.iir.fozzy.lan/account/purchases/my-orders",
            "contragent Full Name", "123456789",
            "i.ivanov@temabit.com", "Пётр Петрович Петров", "+380971111111",
            "+380972222222",
            OrderPaymentType.CourierCash,
            "постучать в дверь три раза",
            Business.Fozzy,
            CreateOrderAndCollectUseCases.CreateOrderStartAndCancel
            )]*/
        public async Task Can_CreateNewEComOrder_and_DoScenario(
            Filials filialId,
            Merchant merchant,
            RequestOrigin origin,
            string owner,
            BasketType basketType,
            string phoneNumber,
            string userId,
            DeliveryAddressType addressType,
            DeliveryType deliveryType,
            string latitude,
            string longitude,
            bool accurate,
            string comments,
            string contactName,
            ItemsCombinations itemsCombinations,
             string accessToken, string url,
            string contragentFullName, string contragentOKPO,
            string customerEmail, string customerName, string customerPhone,
            string feedbackPhone, OrderPaymentType orderPaymentType,
            string notes,
            Business business,
            CreateOrderAndCollectUseCases useCases
            )
        {
            // !!!!!
            var useLocalExpansionService = true; // use local expansion service for debug
            var useTestDb = true; // TEST DB!!!

            var cancellationToken = CancellationToken.None;

            string barcode = Guid.NewGuid().ToString();

            var dateTimeService = DateTimeServiceTest.GetDateTimeService();

            var workflowToExpansionServiceClient = MockHelper.GetWorkflowToExpansionClient(useLocalExpansionService);

            var collectingToExpansionClient = MockHelper.GetCollectingToExpansionClient(useLocalExpansionService); //

            var mappingService = new EntityMapper();

            var transformService = TransformServiceTest.GetTransformService();

            var catalogServiceMock = MockHelper
                .GetCatalogServiceMock(MockHelper.GetCatalogApiClientMock().Object, mappingService);

            var workflowService = EComWorkflowServiceTest.GetEComWorkflowService(useTestDb);

            var application = new CommonAppService(
                workflowService,
                catalogServiceMock.Object,
                transformService,
                useTestDb ? _testExpansionSqlServerDataAccess : _devExpansionSqlServerDataAccess, 
                _mapper,
                LoggerHelper.GetLogger<CommonAppService>().Object
            );

            var timeSlotOn = dateTimeService.Now.AddHours(2);

            var createOrderAndCollectRequest = ExpansionService.GetCreateOrderAndCollectRequest(filialId, merchant, origin,
               owner, basketType, phoneNumber, userId, addressType, deliveryType, latitude, longitude, accurate, comments, contactName,
               itemsCombinations, accessToken, url, contragentFullName, contragentOKPO,
               customerEmail, customerName, customerPhone, feedbackPhone, orderPaymentType, notes, business, timeSlotOn, barcode);

            var fozzyShopCollectingServiceClient = MockHelper.GetFozzyCollectingServiceClient();

            var clientStaffMock = new Mock<IFozzyStaffServiceClient>();

            var fozzyShopWebSiteMock = new Mock<IFozzyShopSiteServiceClient>();

            var collectingService = new FozzyCollectingService(
                useTestDb ? _testExpansionSqlServerDataAccess : _devExpansionSqlServerDataAccess, 
                transformService,
                fozzyShopCollectingServiceClient,
                clientStaffMock.Object,
                fozzyShopWebSiteMock.Object,
                _mapper
                );

            createOrderAndCollectRequest.UseCases = useCases;
            
            var clientToCreateBasket = MockHelper.GetBasketApiClient();
            var basketService = new BasketService(clientToCreateBasket, LoggerHelper.GetLogger<BasketService>().Object);
            var expansionService = new ExpansionService(
                dateTimeService,
                basketService,
                // mocks
                workflowToExpansionServiceClient, 
                transformService,
                collectingService,
                workflowService,
                collectingToExpansionClient,
                application);

            var result = await expansionService
                .DoTestScenarioCreateOrderAndCollectWithReplacements(createOrderAndCollectRequest, cancellationToken);

            if (result.Cancelled)
            {
                Assert.True(result.Success);
            }
            else
            {
                Assert.True(result.Success);
            }
            Assert.NotNull(result.NewEComOrder);
            Assert.Equal(BasketState.Closed, result.NewEComOrder.State);
        }



        private async Task CheckResult(CreateOrderAndCollectResponse result)
        {
            await Task.Delay(0);
        }


        
        [Theory]
        [InlineData("aeaa9279-0aae-4ed7-9406-636516c1b81e")]
        public async Task<OrderData> SendEComOrderToExpansion(Guid basketId)
        {
            var cancellationToken = CancellationToken.None;
            var loggerMock = new Mock<ILogger<WorkflowController>>();
            var useTestDb = false;

            using (var httpClient = HttpClientHelper.GetHttpClient(BaseUrl_Valid, Timeout))
            {
                var options = new MockOptions<BasketServiceOptions>(
                new BasketServiceOptions()
                {
                    BaseUrl = BaseUrl_Valid,
                    AddItemMethodFormat = AddItemMethodFormat,
                    ChangeFilialMethodFormat = ChangeFilialMethodFormat,
                    ChangeOwnerMethodFormat = ChangeOwnerMethodFormat,
                    CloseBasketMethodFormat = CloseBasketMethodFormat,
                    CreateBasketMethod = CreateBasketMethod,
                    GetBasketMethodFormat = GetBasketMethodFormat,
                    GetTimeSlotMethodFormat = GetTimeSlotMethodFormat,
                    Timeout = Timeout,
                    GetCollactableOrderMethodFormat = GetCollactableOrderMethodFormat,
                    UpdateCollectingMethodFormat = UpdateCollectingMethodFormat
                });

                var clientToSendOrderFromEComToExpansion = new BasketApiClient(httpClient, options, LoggerHelper.GetLogger<BasketApiClient>().Object);

                var info = await clientToSendOrderFromEComToExpansion.GetEComOrderForCollecting(basketId, cancellationToken);
                Assert.NotNull(info);


                var workFlowController = new WorkflowController(loggerMock.Object, _mapper);

                var loggerCommandHandlerMock = new Mock<ILogger<CreateOrderComandHandler>>();

               

                var mappingService = new EntityMapper();
                //mappingServiceMock.Setup(x => x.GetDeliveryTypeFromFozzyToEcom(It.IsAny<int>()))
                //    .Returns((int)DeliveryType.DeliveryHome);

                var transformService = TransformServiceTest.GetTransformService();
                

                var workflowService = EComWorkflowServiceTest.GetEComWorkflowService(useTestDb);

                var catalogServiceMock = MockHelper
                    .GetCatalogServiceMock(MockHelper.GetCatalogApiClientMock().Object, mappingService);


                var mediatorMock = new Mock<IMediator>();

                var createOrderCommandHandler = new CreateOrderComandHandler(loggerCommandHandlerMock.Object,
                    workflowService, transformService, catalogServiceMock.Object, mediatorMock.Object);

                var loggerAppMock = new Mock<ILogger<CommonAppService>>();

                var application = new CommonAppService(workflowService,
                    catalogServiceMock.Object, transformService, _devExpansionSqlServerDataAccess, // DEV DB
                   // _expansionSqlServerReadOnlyDataAccess,
                    _mapper, loggerAppMock.Object);

                var view = await workFlowController.EnterCollecting(info, createOrderCommandHandler, cancellationToken);
                Assert.NotNull(view);

                var fozzyShopCollectingServiceClient = MockHelper.GetFozzyCollectingServiceClient();

                var clientStaffMock = new Mock<IFozzyStaffServiceClient>();

                var fozzyShopWebSiteMock = new Mock<IFozzyShopSiteServiceClient>();

                var collectingService = new FozzyCollectingService(_devExpansionSqlServerDataAccess,  // DEV DB
                    transformService,
                    fozzyShopCollectingServiceClient, clientStaffMock.Object, fozzyShopWebSiteMock.Object,
                    _mapper);

                var loggerSendCommandHandlerMock = new Mock<ILogger<SendOrderToFozzyShopCollectingServiceComandHandler>>();

                var sendToCollectHandler = new SendOrderToFozzyShopCollectingServiceComandHandler(
                    collectingService, application, loggerSendCommandHandlerMock.Object, mediatorMock.Object);

                var order = await workFlowController.SendOrderToFozzyShopColectingService(view.BasketId,
                    view.OrderStatus, sendToCollectHandler, cancellationToken);
                Assert.NotNull(order);

                return order;
            }
        }


        private IEnumerable<(Items lager, decimal qty, decimal price)> GetItemsByCombinationLocal(ItemsCombinations itemsCombinations)
        {

            switch (itemsCombinations)
            {
                case ItemsCombinations.OneItem:
                    return new (Items, decimal, decimal)[]
                    {
                        (Items.CocaCola, 10, 20.5m),
                    };

                case ItemsCombinations.TwoItems:
                    return new (Items, decimal, decimal)[]
                    {
                        (Items.CocaCola, 10, 20.5m),
                        (Items.Vine, 2, 300.2m)
                    };

                default: return new (Items, decimal, decimal)[0];
            }
        }

    }




    public static class MockHelper
    {

        public static BasketApiClient GetBasketApiClient()
        {
            var httpClient = HttpClientHelper.GetHttpClient(BasketApiTests.BaseUrl_Valid, BasketApiTests.Timeout);
            var options = new MockOptions<BasketServiceOptions>(
            new BasketServiceOptions()
            {
                BaseUrl = BasketApiTests.BaseUrl_Valid,
                AddItemMethodFormat = BasketApiTests.AddItemMethodFormat,
                ChangeFilialMethodFormat = BasketApiTests.ChangeFilialMethodFormat,
                ChangeOwnerMethodFormat = BasketApiTests.ChangeOwnerMethodFormat,
                CloseBasketMethodFormat = BasketApiTests.CloseBasketMethodFormat,
                CreateBasketMethod = BasketApiTests.CreateBasketMethod,
                GetBasketMethodFormat = BasketApiTests.GetBasketMethodFormat,
                GetTimeSlotMethodFormat = BasketApiTests.GetTimeSlotMethodFormat,
                Timeout = BasketApiTests.Timeout,
                GetCollactableOrderMethodFormat = BasketApiTests.GetCollactableOrderMethodFormat,
                UpdateCollectingMethodFormat = BasketApiTests.UpdateCollectingMethodFormat,
            });
            return new BasketApiClient(httpClient, options, LoggerHelper.GetLogger<BasketApiClient>().Object);
        }


        public static IFozzyCollectingServiceClient GetFozzyCollectingServiceClient()
        {
            var loggerclientCollectingMock = new Mock<ILogger<IFozzyCollectingServiceClient>>();

            var collectingServiceOptions = new MockOptions<FozzyShopCollectingServiceOptions>(new FozzyShopCollectingServiceOptions()
            {
                BaseUrl = "https://s-kv-center-x57.officekiev.fozzy.lan:1449/",
                Timeout = TimeSpan.Parse("00:10:30"),
                PutOrderMethod = "/FozzyShopOrderService.svc/PutOrderData",
                GetByOrderMethodFormat = "/FozzyShopOrderService.svc/GetOrderData?orderId={0}"
            });

            var clientCollecting = HttpClientHelper.GetHttpClient(collectingServiceOptions.Value.BaseUrl,
                collectingServiceOptions.Value.Timeout);

            return new FozzyCollectingServiceClient(clientCollecting,
                collectingServiceOptions, loggerclientCollectingMock.Object);
        }



        public static MockOptions<CollectingToExpansionOptions> GetCollectingToExpansionOptions(bool useLocalExpansionService)
        {
            return new MockOptions<CollectingToExpansionOptions>(
                new CollectingToExpansionOptions()
                {
                    BaseUrl = useLocalExpansionService ? TestConsts.Expansion.LocalServiceBaseUrl 
                        : TestConsts.Expansion.TestServiceBaseUrl,
                    Timeout = TimeSpan.FromMinutes(10),
                    ChangeCollectingMethod = "changeCollecting"
                });
        }

        public static MockOptions<WorkflowToExpansionOptions> GetWorkflowToExpansionOptions(bool useLocalExpansionService)
        {
            return new MockOptions<WorkflowToExpansionOptions>(
                new WorkflowToExpansionOptions()
                {
                    BaseUrl = useLocalExpansionService ? TestConsts.Expansion.LocalServiceBaseUrl
                        : TestConsts.Expansion.TestServiceBaseUrl,
                    Timeout = TimeSpan.FromMinutes(10),
                    AccepCollectingMethodFormat = "acceptCollecting/{0}",
                    CancelCollectingMethodFormat = "cancelCollecting/{0}",
                    EnterCollectingMethod = "enterCollecting",
                    SendOrderToFozzyCollectingServiceMethodFormat = "sendOrderToFozzyShopCollectingService/{0}/status/{1}",
                    SendOrderToFozzyWebSiteMethodFormat = "sendOrderToFozzyShopWebSite/{0}/status/{1}",
                });
        }

        public static IWorkflowToExpansionClient GetWorkflowToExpansionClient(bool useLocalExpansionService)
        {
            var options = GetWorkflowToExpansionOptions(useLocalExpansionService);
            var httpClient = HttpClientHelper.GetHttpClient(options.Value.BaseUrl, options.Value.Timeout);
            return new WorkflowToExpansionClient(httpClient, options, LoggerHelper.GetLogger<IWorkflowToExpansionClient>().Object);
        }


        public static ICollectingToExpansionClient GetCollectingToExpansionClient(bool useLocalExpansionService)
        {
            var options = GetCollectingToExpansionOptions(useLocalExpansionService);
            var httpClient = HttpClientHelper.GetHttpClient(options.Value.BaseUrl, options.Value.Timeout);
            return new CollectingToExpansionClient(httpClient, options, LoggerHelper.GetLogger<ICollectingToExpansionClient>().Object);
        }

        public static Mock<ILogger<T>> GetLogge<T>()
        {
            return new Mock<ILogger<T>>();
        }

        public static Mock<ICatalogApiClient> GetCatalogApiClientMock()
        {
            return new Mock<ICatalogApiClient>();
        }

        public static ICatalogApiClient GetCatalogApiClientMock(HttpClient httpClient, IOptions<CatalogApiOptions> options)
        {
            return new CatalogApiClient(httpClient, options);
        }

        public static Mock<EComCatalogService> GetCatalogServiceMock(ICatalogApiClient catalogApiClient, IEntityMappingService mappingService )
        {
            var gn = new TestDataGenerator();
            var catalogServiceMock = new Mock<EComCatalogService>(catalogApiClient, mappingService);
            catalogServiceMock
                .Setup(x => x.GetCatalogItems(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>(), It.IsAny<int>(), It.IsAny<Guid?>()))
                        .ReturnsAsync((int lagerId, int filialId, int merchantId, CancellationToken cancellation, int delivery, Guid? basketId)
                        =>
                        {
                            var ci = new CatalogInfo();
                            ci.Items.Add(gn.Get_Catalog_All_Items().First(x => x.Id == lagerId));
                            return ci;
                        });

            return catalogServiceMock;
        }

       
    }
}
