using ApplicationServices.Implementation.Utils;
using AutoMapper;
using DataAccess.MsSql.Utils;
using System;
using Xunit;

namespace ECom.Expansion.UnitTests
{
    public class DataAccessUnitTests
    {
        [Fact]
        public void Can_MapDbOrder()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
                cfg.AddProfile<ViewMappingProfile>();
            });

            config.AssertConfigurationIsValid();
        }

        [Fact]
        public void Can_ConvertBase64()
        {
            var bytes = new byte[] { 1 };
            var str  = Convert.ToBase64String(bytes);
            var bytes2 = Convert.FromBase64String(str);
            Assert.Equal(bytes, bytes2);
        }
    }
}