using ApplicationServices.Implementation;
using AutoMapper;
using DomainServices.Implementation;
using ECom.Expansion.TestHelpers;
using Expansion.Interfaces.Enums;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ECom.Expansion.UnitTests.Domain
{
    public class TransformEntitiesTests
    {
        TestDataGenerator _gn;
        IMapper _autoMapper;

       

        public TransformEntitiesTests()
        {
            _gn = new TestDataGenerator();
            _autoMapper = new TestAutoMapper().GetMapper();
        }
            [Fact]
        public async Task Can_TransformCollectingInfoTo_New_TraceableOrder()
        {
            CancellationToken cancellationToken = CancellationToken.None;   
            var basketId = Guid.NewGuid();
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

            // TODO Test properties!!

        }

       
    }


    
}
