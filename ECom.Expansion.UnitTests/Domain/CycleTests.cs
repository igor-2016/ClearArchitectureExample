using ApplicationServices.Implementation;
using AutoMapper;
using Catalog.Ecom;
using Catalog.Interfaces;
using Collecting.Interfaces.Enums;
using Collecting.UseCases.Requests;
using DataAccess.Interfaces;
using DomainServices.Implementation;
using DomainServices.Interfaces;
using ECom.Expansion.TestHelpers;
using ECom.Types.Delivery;
using Entities.Models.Expansion;
using Expansion.Interfaces.Enums;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Workflow.Interfaces;
using Xunit;

namespace ECom.Expansion.UnitTests.Domain
{
    public class CycleTests
    {
        TestDataGenerator _gn;
        IMapper _autoMapper;

       

        public CycleTests()
        {
            _gn = new TestDataGenerator();
            _autoMapper = new TestAutoMapper().GetMapper();
        }
        

        [Fact]
        public async Task Can_Pass_Scenario_1()
        {
            var cancellationToken = CancellationToken.None;
            var loggerMock = new Mock<ILogger<CommonAppService>>();
            //var ecomLoggerMock = new Mock<ILogger<IEComAndCollectOrderDifferenceCalculator>>();
            var mappingService = new EntityMapper();
            var dateTimeNow = DateTime.Now;
            var dateService = new DateTimeService(dateTimeNow);
            var transformService = new TransformService(_autoMapper, mappingService, dateService);
            var catalogApiClient = new Mock<ICatalogApiClient>();
            var workflowService = new Mock<IWorkflowService>();


            var basketId = Guid.NewGuid();
            var fozzyOrderId = "12434679";
            
            var initialEComBasket = _gn.CreateEmptyEComBasket(basketId, orderId: fozzyOrderId);

            var line1Id = Guid.NewGuid();   
            var lager1Id = Items.Farba;  // краска
            _gn.AddItemToEComBasket(initialEComBasket, line1Id, lager1Id, 1, 200.3m, 
                "Краска, будет собрана и не меняется");

            //var line4Id = Guid.NewGuid();
            //var lager4Id = Items.CocaCola;         // кола 
            //_gn.AddItemToEComBasket(initialEComBasket, line4Id, lager4Id, 7, 21.5m, 
            //    "Coca-Cola, будет собрана частично и подтверждена ECom на меньшее количество");

            
            var line3Id = Guid.NewGuid();     // 
            var lager3Id = Items.Viski_37400;      // виски
            var replacementForlager3Id = Items.Viski_2712;// виски, замена для 37400
            _gn.AddItemToEComBasket(initialEComBasket, line3Id, lager3Id, 1, 306.2m,
                $"Виски, будет заменено на {replacementForlager3Id}");//, $"{(int)replacementForlager3Id}");

            //замена 1
            
            var line5Id = Guid.NewGuid();
            var lager5Id = Items.Water_599434; //  вода, будет заменена
            var replacementForlager5Id = Items.Water_440815;// вода
            var newFromFozzyLine = _gn.AddItemToEComBasket(initialEComBasket, line5Id, lager5Id, 10, 46.8m,
                $"Вода, будет заменена частично на {replacementForlager5Id}");//, $"{(int)replacementForlager5Id}");


            // этого кэйса нет, сборка только предлагает замены
            //var newFromFozzyLineId = Guid.NewGuid();
            //var newFromFozzyLagerId = 599434; //  вода, будет добавлена как новая при сборке Fozzy ТСД
            //var newFromFozzyLine = _gn.CreateEComItem(newFromFozzyLineId, newFromFozzyLagerId, 2, 10.78m,
            //    "вода, будет добавлена как новая при сборке Fozzy ТСД");

            // создадим новый ордер
            var newTraceableOrder = await transformService.ToNewTraceableOrder(
                initialEComBasket,
                _gn.GetValidCatalogSingleItemInfo,
                _gn.Get_Site_OrderOrigin,
                _gn.DefaultRowNumCalculator,
                _gn.GetLogisticType,
                cancellationToken
                );

            newTraceableOrder.OrderStatus = 913;

            // такой ордер отправим на Fozzy
            var fozzyOrder = transformService.ToFozzyOrder(newTraceableOrder);

            // началась сборка по сценарию
            fozzyOrder.GetOrder().orderStatus = FozzyOrderStatus.Status15.ToInt().ToString();
            //..
            newTraceableOrder.OrderStatus = FozzyOrderStatus.Status15.ToInt();



            #region Update PickedQty
            // 1
            var itemCompletellyFilled = _gn.GetFozzyOrderItemByLagerId(fozzyOrder, Items.Farba);
            Assert.NotNull(itemCompletellyFilled);
            itemCompletellyFilled.pickerQuantity = itemCompletellyFilled.orderQuantity;

            // 2
            //var itemPartialFilled = _gn.GetFozzyOrderItemByLagerId(fozzyOrder, Items.CocaCola);
            //Assert.NotNull(itemPartialFilled);
            //itemPartialFilled.pickerQuantity = (itemPartialFilled.orderQuantity.ToDecimal() - 2).ToDecimalString();


            // 3
            var itemWithReplacement = _gn.GetFozzyOrderItemByLagerId(fozzyOrder, Items.Viski_37400);
            Assert.NotNull(itemWithReplacement);
            itemWithReplacement.pickerQuantity = "0"; // нужно замены
            itemWithReplacement.replacementLagers = $"{(int)replacementForlager3Id}";

            // 4
            var itemPartialFilled = _gn.GetFozzyOrderItemByLagerId(fozzyOrder, Items.Water_599434);
            Assert.NotNull(itemPartialFilled);
            itemPartialFilled.pickerQuantity = (itemPartialFilled.orderQuantity.ToDecimal() - 3).ToDecimalString(); // нужно замены
            itemPartialFilled.replacementLagers = $"{(int)replacementForlager5Id}";

            #endregion Update PickedQty

            // необходимо согласование
            fozzyOrder.GetOrder().orderStatus = FozzyOrderStatus.Status914.ToInt().ToString();


            // ордер для сравнения с существующим 
            var changedTraceableOrder = await transformService.FromFozzyOrder(
                newTraceableOrder,
                newTraceableOrder.GetPicker(),
                fozzyOrder,
                _gn.GetValidCatalogSingleItemInfo,
                _gn.Get_Valid_PickerInfoByInn,
                _gn.NoCheckFilledCalculator,
                _gn.DefaultRowNumCalculator,
                transformService.GetLogisticType,
                cancellationToken
                );

            // сравниваем

            #region Подготовим моки

            var mappingServiceMock = new Mock<IEntityMappingService>();
            mappingServiceMock.Setup(x => x.GetDeliveryTypeFromFozzyToEcom(It.IsAny<int>()))
                .Returns((int)DeliveryType.DeliveryHome);

            var dataAccessMock = new Mock<IDataAccess>();
            var readOnlyDataAccessMock = new Mock<IReadOnlyDataAccess>();

            var catalogService = new Mock<EComCatalogService>(catalogApiClient.Object, mappingServiceMock.Object);
            catalogService
                .Setup(x => x.GetCatalogItem(It.IsAny<TraceableOrder>(), It.IsAny<int?>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((TraceableOrder x, int? lagerId, CancellationToken cancellation) =>
                    _gn.Get_Catalog_All_Items().First(x => x.Id == lagerId.Value));

            var application = new CommonAppService(workflowService.Object,
                catalogService.Object, transformService, dataAccessMock.Object, //readOnlyDataAccessMock.Object, 
                    _autoMapper, loggerMock.Object);

            #endregion Подготовим моки

            var updatedOrderAfterReplacements = await application.OfferReplacements(new CollectableOrdersInput()
            {
                OrderToBeUpdated = newTraceableOrder,
                SourceOfChanges = changedTraceableOrder
            }, cancellationToken);

            Assert.Equal(5, updatedOrderAfterReplacements.Items.Count);
            var farb = _gn.GetTraceableOrderItemByLagerId(updatedOrderAfterReplacements, Items.Farba);
            Assert.Equal(1, farb.PickerQuantity);

            var viski = _gn.GetTraceableOrderItemByLagerId(updatedOrderAfterReplacements, Items.Viski_37400);
            Assert.Equal(0, viski.PickerQuantity);

            var water = _gn.GetTraceableOrderItemByLagerId(updatedOrderAfterReplacements, Items.Water_599434);
            Assert.Equal(7, water.PickerQuantity);

            // new (замены)
            var viski_2 = _gn.GetTraceableOrderItemByLagerId(updatedOrderAfterReplacements, Items.Viski_2712);
            Assert.Equal(null, viski_2.PickerQuantity);
            Assert.Equal(0, viski_2.OrderQuantity);

            var water_2 = _gn.GetTraceableOrderItemByLagerId(updatedOrderAfterReplacements, Items.Water_440815);
            Assert.Equal(null, water_2.PickerQuantity);
            Assert.Equal(0, water_2.OrderQuantity);
            
            //... проверить с Иваном
            var offerReplacements = transformService.ToBasketCollectingItems(updatedOrderAfterReplacements);

            // отправляем на подтверждение
            //...
            // оператор вводит количество заказа для замен и добавляет новую позицию
            
            // подготовить подтверждение оператор
            var cigaQty = 4;
            var newViskyQty = 1;
            var newWaterQty = 5;

            // новая позиция от оператора
            var newFromEComlineId = Guid.NewGuid();
            var newFromEComLagerId = Items.Cigarettes_255349; // сигареты
            var newEComLine = _gn.CreateEComItem(newFromEComlineId, newFromEComLagerId, cigaQty, 3.7m,
                "сигареты, новая позиция при подтверждении замен из ECom");


            var acceptedEComBasket = _gn.CreateEmptyEComBasket(basketId, orderId: fozzyOrderId);
            foreach (var item in initialEComBasket.CollectableItems)
            {
                _gn.AddItemToEComBasket(acceptedEComBasket, item);
            }
            _gn.AddItemToEComBasket(acceptedEComBasket, Guid.NewGuid(), Items.Viski_2712, newViskyQty, 500m, "замена");
            _gn.AddItemToEComBasket(acceptedEComBasket, Guid.NewGuid(), Items.Water_440815, newWaterQty, 40m, "замена");
            _gn.AddItemToEComBasket(acceptedEComBasket, newEComLine); //4, 

            var cigaAccepted = acceptedEComBasket.CollectableItems.First(x => x.LagerId == (int)Items.Cigarettes_255349);
            Assert.Equal(cigaQty, cigaAccepted.Qty);
            var viskyAccepted = acceptedEComBasket.CollectableItems.First(x => x.LagerId == (int)Items.Viski_2712);
            Assert.Equal(newViskyQty, viskyAccepted.Qty);
            var waterAccepted = acceptedEComBasket.CollectableItems.First(x => x.LagerId == (int)Items.Water_440815);
            Assert.Equal(newWaterQty, waterAccepted.Qty);

            var waterPartialFilledAccepted = acceptedEComBasket.CollectableItems.First(x => x.LagerId == (int)Items.Water_599434);
            waterPartialFilledAccepted.Qty = water.PickerQuantity ?? 0; // 7

            // получаем подтверждённые
            var acceptedOrder = await transformService.FromAcceptedOrder(
                acceptedEComBasket,
                updatedOrderAfterReplacements,
                _gn.GetValidCatalogSingleItemInfo,
                _gn.DefaultRowNumCalculator,
                _gn.NoCheckFilledCalculator,
                _gn.GetLogisticType,
                cancellationToken
                );

            

            Assert.NotNull(acceptedOrder);
            farb = _gn.GetTraceableOrderItemByLagerId(acceptedOrder, Items.Farba);
            Assert.Equal(1, farb.OrderQuantity);
            Assert.Equal(1, farb.PickerQuantity);

            viski = _gn.GetTraceableOrderItemByLagerId(acceptedOrder, Items.Viski_37400);
            Assert.Equal(1, viski.OrderQuantity);
            Assert.Equal(0, viski.PickerQuantity);

            water = _gn.GetTraceableOrderItemByLagerId(acceptedOrder, Items.Water_599434);
            Assert.Equal(7, water.OrderQuantity); // not 10
            Assert.Equal(7, water.PickerQuantity);

            // new (замены)
            viski_2 = _gn.GetTraceableOrderItemByLagerId(acceptedOrder, Items.Viski_2712);
            Assert.Equal(1, viski_2.OrderQuantity);
            Assert.Equal(null, viski_2.PickerQuantity);

            water_2 = _gn.GetTraceableOrderItemByLagerId(acceptedOrder, Items.Water_440815);
            Assert.Equal(5, water_2.OrderQuantity);
            Assert.Equal(null, water_2.PickerQuantity);

            // new
            var ciga = _gn.GetTraceableOrderItemByLagerId(acceptedOrder, Items.Cigarettes_255349);
            Assert.Equal(4, ciga.OrderQuantity);
            Assert.Equal(null, ciga.PickerQuantity);

            // применяем изменения от ECom через Workflow (от оператора)

            var updatedFromEcomAllOrder = await application.AcceptReplacements(new CollectableOrdersInput()
            {
                OrderToBeUpdated = updatedOrderAfterReplacements,
                SourceOfChanges = acceptedOrder
            }, cancellationToken);

            Assert.Equal(6, updatedFromEcomAllOrder.Items.Count);

            farb = _gn.GetTraceableOrderItemByLagerId(updatedFromEcomAllOrder, Items.Farba);
            Assert.Equal(1, farb.OrderQuantity);
            Assert.Equal(1, farb.PickerQuantity);

            viski = _gn.GetTraceableOrderItemByLagerId(updatedFromEcomAllOrder, Items.Viski_37400);
            Assert.Equal(1, viski.OrderQuantity);
            Assert.Equal(0, viski.PickerQuantity);

            water = _gn.GetTraceableOrderItemByLagerId(updatedFromEcomAllOrder, Items.Water_599434);
            Assert.Equal(7, water.OrderQuantity);//10
            Assert.Equal(7, water.PickerQuantity);

            // new (замены)
            viski_2 = _gn.GetTraceableOrderItemByLagerId(updatedFromEcomAllOrder, Items.Viski_2712);
            Assert.Equal(1, viski_2.OrderQuantity);
            Assert.Equal(null, viski_2.PickerQuantity);

            water_2 = _gn.GetTraceableOrderItemByLagerId(updatedFromEcomAllOrder, Items.Water_440815);
            Assert.Equal(5, water_2.OrderQuantity);
            Assert.Equal(null, water_2.PickerQuantity);

            // new
            ciga = _gn.GetTraceableOrderItemByLagerId(updatedFromEcomAllOrder, Items.Cigarettes_255349);
            Assert.Equal(4, ciga.OrderQuantity);
            Assert.Equal(null, ciga.PickerQuantity);

            // отправим на сборку

            //----------------------------------------------------------------------------
            updatedFromEcomAllOrder.OrderStatus = 915;

            // такой ордер отправим на Fozzy
            var fozzyOrder2 = transformService.ToFozzyOrder(updatedFromEcomAllOrder);

            // началась сборка по сценарию
            fozzyOrder2.GetOrder().orderStatus = FozzyOrderStatus.Status15.ToInt().ToString();
            //..
            updatedFromEcomAllOrder.OrderStatus = FozzyOrderStatus.Status15.ToInt();



            #region Update PickedQty
            // 1
            var itemCiga = _gn.GetFozzyOrderItemByLagerId(fozzyOrder2, Items.Cigarettes_255349);
            Assert.NotNull(itemCiga);
            itemCiga.pickerQuantity = itemCiga.orderQuantity; 
            
            // 2
            var itemWater44 = _gn.GetFozzyOrderItemByLagerId(fozzyOrder2, Items.Water_440815);
            Assert.NotNull(itemWater44);
            itemWater44.pickerQuantity = itemWater44.orderQuantity;
            
            // 3
            var itemWiski27 = _gn.GetFozzyOrderItemByLagerId(fozzyOrder2, Items.Viski_2712);
            Assert.NotNull(itemWiski27);
            itemWiski27.pickerQuantity = itemWiski27.orderQuantity;

            

            #endregion Update PickedQty

            // завершаем сборку
            fozzyOrder2.GetOrder().orderStatus = FozzyOrderStatus.Status911.ToInt().ToString();


            // ордер для сравнения с существующим 
            var doneOrder = await transformService.FromFozzyOrder(
                updatedFromEcomAllOrder,
                updatedFromEcomAllOrder.GetPicker(),
                fozzyOrder2,
                _gn.GetValidCatalogSingleItemInfo,
                _gn.Get_Valid_PickerInfoByInn,
                _gn.NoCheckFilledCalculator,
                _gn.DefaultRowNumCalculator,
                transformService.GetLogisticType,
                cancellationToken
                );

            //...
            // применим результаты сборки
            Assert.NotNull(doneOrder);
            farb = _gn.GetTraceableOrderItemByLagerId(doneOrder, Items.Farba);
            Assert.Equal(1, farb.OrderQuantity);
            Assert.Equal(1, farb.PickerQuantity);

            viski = _gn.GetTraceableOrderItemByLagerId(doneOrder, Items.Viski_37400);
            Assert.Equal(1, viski.OrderQuantity);
            Assert.Equal(0, viski.PickerQuantity);

            water = _gn.GetTraceableOrderItemByLagerId(doneOrder, Items.Water_599434);
            Assert.Equal(7, water.OrderQuantity); // not 10
            Assert.Equal(7, water.PickerQuantity);

            // new (замены)
            viski_2 = _gn.GetTraceableOrderItemByLagerId(doneOrder, Items.Viski_2712);
            Assert.Equal(1, viski_2.OrderQuantity);
            Assert.Equal(1, viski_2.PickerQuantity);

            water_2 = _gn.GetTraceableOrderItemByLagerId(doneOrder, Items.Water_440815);
            Assert.Equal(5, water_2.OrderQuantity);
            Assert.Equal(5, water_2.PickerQuantity);

            // new
            ciga = _gn.GetTraceableOrderItemByLagerId(doneOrder, Items.Cigarettes_255349);
            Assert.Equal(4, ciga.OrderQuantity);
            Assert.Equal(4, ciga.PickerQuantity);


            // применим изменения
            var updatedFromFozzyAllOrder = await application.AcceptCollecting(new CollectableOrdersInput()
            {
                OrderToBeUpdated = updatedFromEcomAllOrder,
                SourceOfChanges = doneOrder
            }, cancellationToken);

            Assert.Equal(6, updatedFromFozzyAllOrder.Items.Count);

            farb = _gn.GetTraceableOrderItemByLagerId(updatedFromFozzyAllOrder, Items.Farba);
            Assert.Equal(1, farb.OrderQuantity);
            Assert.Equal(1, farb.PickerQuantity);

            viski = _gn.GetTraceableOrderItemByLagerId(updatedFromFozzyAllOrder, Items.Viski_37400);
            Assert.Equal(1, viski.OrderQuantity);
            Assert.Equal(0, viski.PickerQuantity);

            water = _gn.GetTraceableOrderItemByLagerId(updatedFromFozzyAllOrder, Items.Water_599434);
            Assert.Equal(7, water.OrderQuantity);//10
            Assert.Equal(7, water.PickerQuantity);

            // new (замены)
            viski_2 = _gn.GetTraceableOrderItemByLagerId(updatedFromFozzyAllOrder, Items.Viski_2712);
            Assert.Equal(1, viski_2.OrderQuantity);
            Assert.Equal(1, viski_2.PickerQuantity);

            water_2 = _gn.GetTraceableOrderItemByLagerId(updatedFromFozzyAllOrder, Items.Water_440815);
            Assert.Equal(5, water_2.OrderQuantity);
            Assert.Equal(5, water_2.PickerQuantity);

            // new
            ciga = _gn.GetTraceableOrderItemByLagerId(updatedFromFozzyAllOrder, Items.Cigarettes_255349);
            Assert.Equal(4, ciga.OrderQuantity);
            Assert.Equal(4, ciga.PickerQuantity);

            // всё.
        }


    }


    
}
