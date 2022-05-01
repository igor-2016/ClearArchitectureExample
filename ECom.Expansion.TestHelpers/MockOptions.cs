using Microsoft.Extensions.Options;

namespace ECom.Expansion.TestHelpers
{
    public class MockOptions<T> : IOptions<T> where T : class, new()
    {
        private readonly T _cfg;

        public MockOptions(T cfg)
        {
            _cfg = cfg;
        }
        public T Value => _cfg;
    }
}