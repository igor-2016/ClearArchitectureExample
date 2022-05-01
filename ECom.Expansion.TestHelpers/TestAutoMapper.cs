using ApplicationServices.Implementation.Utils;
using AutoMapper;
using DataAccess.MsSql.Utils;
using HealthCheck.Controllers;

namespace ECom.Expansion.TestHelpers
{
    public class TestAutoMapper
    {
        public IMapper GetMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
               // cfg.
                cfg.AddProfile<MappingProfile>();
                cfg.AddProfile<ViewMappingProfile>();
                cfg.AddProfile<HealthcheckControllerMapping>();
            });

            config.AssertConfigurationIsValid();
            return new Mapper(config);
        }
    }
}
