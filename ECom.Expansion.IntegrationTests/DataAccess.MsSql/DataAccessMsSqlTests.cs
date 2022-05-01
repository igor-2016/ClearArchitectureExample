using ApplicationServices.Implementation;
using AutoMapper;
using DataAccess.Interfaces;
using DataAccess.Interfaces.Dto;
using DataAccess.Interfaces.Specifications;
using DataAccess.MsSql;
using DataAccess.MsSql.Utils;
using DomainServices.Implementation;
using ECom.Expansion.TestHelpers;
using Entities.Models.Expansion;
using Expansion.Interfaces.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ECom.Expansion.IntegrationTests
{
    public class DataAccessMsSqlTests
    {
        readonly IMapper _mapper;
        readonly DbContextOptions<AppDbContext> _inMemoryDbOption;
        readonly DbContextOptions<AppDbContext> _sqlServerOptions;
        readonly IDataAccess _expansionInMemoryDbDataAccess;
        readonly IDataAccess _expansionSqlServerDataAccess;

        readonly TestDataGenerator _gn;
        readonly IMapper _autoMapper;
        readonly IConfiguration _config;

        public DataAccessMsSqlTests()
        {
            _config = ConfigHelper.InitConfiguration();

            _gn = new TestDataGenerator();
            _autoMapper = new TestAutoMapper().GetMapper();

            _inMemoryDbOption = new DbContextOptionsBuilder<AppDbContext>()
                    .UseInMemoryDatabase(databaseName: "EComExpansionDev")
                    .Options;

            _sqlServerOptions = new DbContextOptionsBuilder<AppDbContext>()
                    .UseSqlServer(_config.GetConnectionString("MsSqlDev"))
                    .Options;

            
            _expansionInMemoryDbDataAccess = new AppDbContext(_inMemoryDbOption, _autoMapper);
            _expansionSqlServerDataAccess = new AppDbContext(_sqlServerOptions, _autoMapper);
        }

        

        [Fact]
        public async Task Can_CreateTraceableOrder()
        {
            var dataAccess = _expansionSqlServerDataAccess; //_expansionInMemoryDbDataAccess;
            var basketId = Guid.NewGuid();
            var order = await Get_New_TraceableOrder(basketId);
            

            var createdOrder = await dataAccess.AddNewAsync<TraceableOrder, TraceableOrderDto, Guid>(order);
            Assert.NotNull(createdOrder);
            Assert.Equal(order.Id, createdOrder.Id); 

            var orderFromDb = await dataAccess.GetByIdWithoutIncludes<TraceableOrder, TraceableOrderDto, Guid>(createdOrder.Id);
            Assert.NotNull(orderFromDb);
            Assert.NotNull(orderFromDb.RowVersion);
            Assert.Equal(order.Id, orderFromDb.Id);
        }

        
        [Fact]
        public async Task Can_UpdateOrder()
        {
            var dataAccess = _expansionSqlServerDataAccess;
            
            var basketId = Guid.NewGuid();
            var order = await Get_New_TraceableOrder(basketId);
            order.OrderNumber = "00002";

           
            var createdOrder = await dataAccess.AddNewAsync<TraceableOrder, TraceableOrderDto, Guid>(order);
            Assert.NotNull(createdOrder);
            Assert.Equal(order.Id, createdOrder.Id);
            Assert.Equal("00002", createdOrder.OrderNumber);
            Assert.Equal(order.Items.Count, createdOrder.Items.Count);


            var newNumber = "string>createdOrder.Number;";
            createdOrder.OrderNumber = newNumber;

            var updatedLagerId = order.Items[0].LagerId;
            var oldLagerName = order.Items[0].LagerName;
            var newLagerName = oldLagerName + " (updated) ";
            createdOrder.Items[0].LagerName = newLagerName;

            var spec = new GetOrderByOrdeIdWithItems(createdOrder.Id);
            var updatedOrder = await dataAccess.UpdateAsync<TraceableOrder, TraceableOrderDto, Guid>(createdOrder, spec);
            Assert.NotNull(updatedOrder);
            Assert.Equal(order.Id, updatedOrder.Id);
            Assert.Equal(newNumber, updatedOrder.OrderNumber);
            Assert.Equal(order.Items.Count, updatedOrder.Items.Count);
            var item = updatedOrder.Items.First(x => x.LagerId == updatedLagerId);
            Assert.Equal(newLagerName, item.LagerName);

            var reloadedOrder = await dataAccess.GetById<TraceableOrder, TraceableOrderDto, Guid>(spec);
            Assert.NotNull(reloadedOrder);
            Assert.Equal(order.Id, reloadedOrder.Id);
            Assert.Equal(newNumber, reloadedOrder.OrderNumber);
            Assert.Equal(order.Items.Count, reloadedOrder.Items.Count);
            item = updatedOrder.Items.First(x => x.LagerId == updatedLagerId);
            Assert.Equal(newLagerName, item.LagerName);
        }

        /*
        [Theory]
        [InlineData("0E3CFF26-F630-4961-BE7C-98704EC177BD", 2)]
        public async Task Can_LoadItems(string orderIdAsString, int itemsCount)
        {
            var dataAccess = _expansionSqlServerDataAccess;

            var orderId = Guid.Parse(orderIdAsString);
            var spec = new GetOrderByOrdeIdWithItems(orderId);
            var reloadedOrder = await dataAccess.GetById<TraceableOrder, TraceableOrderDto, Guid>(spec);
            Assert.NotNull(reloadedOrder);
            Assert.Equal(orderId, reloadedOrder.Id);
            Assert.Equal(itemsCount, reloadedOrder.Items.Count);
        }
        */


        [Fact]
        public async Task Can_DeleteOrder()
        {
            var dataAccess = _expansionSqlServerDataAccess;
            var basketId = Guid.NewGuid();
            var order = await Get_New_TraceableOrder(basketId);
            order.OrderNumber = "00003";
           

            var createdOrder = await dataAccess.AddNewAsync<TraceableOrder, TraceableOrderDto, Guid>(order);
            Assert.NotNull(createdOrder);
            Assert.Equal(order.Id, createdOrder.Id);
            Assert.Equal("00003", createdOrder.OrderNumber);

            await dataAccess.DeleteAsync<TraceableOrderDto, Guid>(createdOrder.Id);

            var ex = await Assert.ThrowsAsync<DataAccessException>(() =>
                dataAccess.GetByIdWithoutIncludes<TraceableOrder, TraceableOrderDto, Guid>(createdOrder.Id));

            Assert.Equal((int)DbErrors.EntityNotFound, ex.ErrorCode);
        }
      
        [Fact]
        public async Task Can_CheckOrderVersionChangeInMemoryDb()
        {
            var dataAccess = _expansionInMemoryDbDataAccess;
            var basketId = Guid.NewGuid();
            var order = await Get_New_TraceableOrder(basketId);
            order.OrderNumber = order.ToString();
            

            var createdOrder = await dataAccess.AddNewAsync<TraceableOrder, TraceableOrderDto, Guid>(order);
            Assert.NotNull(createdOrder);
            Assert.Equal(order.Id, createdOrder.Id);



            var orderFromDb = await dataAccess.GetByIdWithoutIncludes<TraceableOrder, TraceableOrderDto, Guid>(order.Id);
            Assert.NotNull(orderFromDb);
            Assert.Equal(order.Id, orderFromDb.Id);

            orderFromDb.OrderNumber = "100";
            var spec = new GetOrderByOrdeIdNoItems(order.Id); // no items
            await dataAccess.UpdateAsync<TraceableOrder, TraceableOrderDto, Guid>(orderFromDb, spec);
            var orderFromDb2 = await dataAccess.GetByIdWithoutIncludes<TraceableOrder, TraceableOrderDto, Guid>(order.Id);
            createdOrder.RowVersion = "AQ=="; // in memory db does not support row version
            Assert.Equal(order.Id, orderFromDb2.Id);
            Assert.NotEqual(createdOrder.RowVersion, orderFromDb2.RowVersion);

            await Assert.ThrowsAsync<DataAccessException>(() =>
                dataAccess.UpdateAsync<TraceableOrder, TraceableOrderDto, Guid>(createdOrder, spec));

            createdOrder.RowVersion = "";
            await dataAccess.UpdateAsync<TraceableOrder, TraceableOrderDto, Guid>(createdOrder, spec);
        }
        
        [Fact]
       public async Task Can_CheckOrderVersionChangeSqlServer()
        {
            var dataAccess = _expansionSqlServerDataAccess;
            var basketId = Guid.NewGuid();
            var order = await Get_New_TraceableOrder(basketId);
            order.OrderNumber = order.ToString();

            var createdOrder = await dataAccess.AddNewAsync<TraceableOrder, TraceableOrderDto, Guid>(order);
            Assert.NotNull(createdOrder);
            Assert.Equal(order.Id, createdOrder.Id);



            var orderFromDb = await dataAccess.GetByIdWithoutIncludes<TraceableOrder, TraceableOrderDto, Guid>(order.Id);
            Assert.NotNull(orderFromDb);
            Assert.Equal(order.Id, orderFromDb.Id);

            orderFromDb.OrderNumber = "100";
            var spec = new GetOrderByOrdeIdNoItems(order.Id);
            var updatedOrderBeforeOld = await dataAccess.UpdateAsync<TraceableOrder, TraceableOrderDto, Guid>(orderFromDb, spec);
            var orderFromDb2 = await dataAccess.GetByIdWithoutIncludes<TraceableOrder, TraceableOrderDto, Guid>(order.Id);
            Assert.Equal(order.Id, orderFromDb2.Id);
            Assert.NotEqual(createdOrder.RowVersion, orderFromDb2.RowVersion);

            var ex = await Assert.ThrowsAsync<DataAccessException>(() =>
                dataAccess.UpdateAsync<TraceableOrder, TraceableOrderDto, Guid>(createdOrder, spec));
            
            Assert.Equal((int)DbErrors.InvalidEntityVersion, ex.ErrorCode);

            createdOrder.RowVersion = updatedOrderBeforeOld.RowVersion;
            await dataAccess.UpdateAsync<TraceableOrder, TraceableOrderDto, Guid>(createdOrder, spec);
        }
        


        private async Task<TraceableOrder> Get_New_TraceableOrder(Guid basketId)
        {
            CancellationToken cancellationToken = CancellationToken.None;
            //var basketId = Guid.NewGuid();
            var line1Id = Guid.NewGuid();
            var line2Id = Guid.NewGuid();
            var lager1Id = Items.CocaCola;
            var lager2Id = Items.Vine;

            var eComOrderInfo = _gn.CreateEComBasketWithTwoLines(basketId, line1Id, line2Id,
                lager1Id: lager1Id, lager2Id: lager2Id, orderId: "12434679");

            var mappingService = new EntityMapper();
            var dateTimeNow = DateTime.Now;
            var dateService = new DateTimeService(dateTimeNow);
            var transformService = new TransformService(_autoMapper, mappingService, dateService);

            var traceableOrder = await transformService.ToNewTraceableOrder(
                eComOrderInfo,
                _gn.GetValidCatalogSingleItemInfo,
                _gn.Get_Site_OrderOrigin,
                _gn.DefaultRowNumCalculator,
                _gn.GetLogisticType,
                cancellationToken
                );

            Assert.NotNull(traceableOrder);

            return traceableOrder;

        }
    }
}