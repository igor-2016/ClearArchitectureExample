using Microsoft.Extensions.Logging;
using Moq;

namespace ECom.Expansion.TestHelpers
{
    public static class LoggerHelper
    {
        public static Mock<ILogger<T>> GetLogger<T>()
        {
            return new Mock<ILogger<T>>();
        }
    }
}
