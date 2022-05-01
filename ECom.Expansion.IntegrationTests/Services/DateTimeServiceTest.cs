using DomainServices.Implementation;
using Xunit;

namespace ECom.Expansion.IntegrationTests.Services
{
    public class DateTimeServiceTest
    {
        private static DateTimeService _dateTimeService;

        public static DateTimeService GetDateTimeService()
        {
            if (_dateTimeService == null)
            {
                _dateTimeService = new DateTimeService();
            }
            return _dateTimeService;
        }

        [Fact]
        public void GetMapperTest()
        {
            var context = GetDateTimeService();
            Assert.IsType<DateTimeService>(context);
        }
    }
}
