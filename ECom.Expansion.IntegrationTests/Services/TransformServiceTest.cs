using DomainServices.Implementation;
using Xunit;

namespace ECom.Expansion.IntegrationTests.Services
{
    public class TransformServiceTest
    {
        private static TransformService _transformService;

        public static TransformService GetTransformService()
        {
            if (_transformService == null)
            {
                _transformService = new TransformService(MapperTest.GetMapper(), MapperTest.GetMappingService(), DateTimeServiceTest.GetDateTimeService());
            }
            return _transformService;
        }

        [Fact]
        public void GetTransformServiceTest()
        {
            var transformService = GetTransformService();
            Assert.IsType<TransformService>(transformService);
        }
    }
}
