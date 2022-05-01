using ECom.Expansion.TestHelpers;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ECom.Expansion.UnitTests.Logger
{
    public class LoggerTest
    {
        [Fact]
        public void MockLogger()
        {
            var loger = LoggerHelper.GetLogger<TestClass>();
            Assert.IsType<Mock<ILogger<TestClass>>>(loger);
        }

        public class TestClass
        {
            public int Test { get; set; }
        };
    }
}
