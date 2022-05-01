using Utils.Exceptions;
namespace Utils.Extensions
{
    public static class ExceptionExtensions
    {
        public static SourceException AddRequestedTarget(this SourceException ex, string target)
        {
            ex.ExceptionTarget = target;
            return ex;
        }
    }
}
