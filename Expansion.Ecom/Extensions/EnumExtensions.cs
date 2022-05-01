using Expansion.Interfaces.Dto;

namespace Expansion.Ecom.Extensions
{
    public static class EnumExtensions
    {
        public static bool Allow (this CreateOrderAndCollectUseCases useCases, CreateOrderAndCollectUseCases toCompareWith)
        {
            return ((int)useCases & (int)toCompareWith) > 0;
        }
    }
}
