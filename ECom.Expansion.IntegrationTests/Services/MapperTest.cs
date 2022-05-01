using ApplicationServices.Implementation;
using AutoMapper;
using ECom.Expansion.TestHelpers;
using Xunit;

namespace ECom.Expansion.IntegrationTests.Services
{   
    public class MapperTest
    {
        private static IMapper _mapper;
        private static EntityMapper _mappingService;

        public static IMapper GetMapper()
        {
            if (_mapper == null)
            {
                _mapper = new TestAutoMapper().GetMapper();
            }
            return _mapper;
        }

        public static EntityMapper GetMappingService()
        {
            if (_mappingService == null)
            {
                _mappingService = new EntityMapper();
            }
            return _mappingService;
        }

        [Fact]
        public void GetMapperTest()
        {
            var mapper = GetMapper();
            Assert.IsType<Mapper>(mapper);
        }

        [Fact]
        public void GetMappingServiceTest()
        {
            var mappingService = GetMappingService();
            Assert.IsType<EntityMapper>(mappingService);
        }

    }
}
