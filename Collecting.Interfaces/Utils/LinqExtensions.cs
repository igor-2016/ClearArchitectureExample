namespace Collecting.Interfaces
{
    public static class LinqExtensions
    {
        public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> src)
        {
            return src ?? Enumerable.Empty<T>();
        }

        public static IEnumerable<T> NullIfEmpty<T>(this IEnumerable<T> src)
        {
            return src.EmptyIfNull().Any() ? src : null;
        }

        public static IOrderedEnumerable<T> OrderBy<T>(this IEnumerable<T> src, Func<IEnumerable<T>, IOrderedEnumerable<T>> sorter)
        {
            return sorter(src);
        }

        public static bool In<T>(this T val, params T[] items)
        {
            return items.EmptyIfNull().Contains(val);
        }

        public static bool Between(this int val, int min, int max)
        {
            return (val >= min) && (val <= max);
        }

        public static bool Between(this decimal val, decimal min, decimal max)
        {
            return (val >= min) && (val <= max);
        }

        public static bool Between(this double val, double min, double max)
        {
            return (val >= min) && (val <= max);
        }

        public static bool Between(this DateTime val, DateTime min, DateTime max)
        {
            return (val >= min) && (val <= max);
        }

        public static bool Between(this TimeSpan val, TimeSpan min, TimeSpan max)
        {
            return (val >= min) && (val <= max);
        }

        public static bool IsNull<T>(this T val) where T : class
        {
            return (val == null);
        }

        public static bool IsDefault<T>(this T val)
        {
            return Equals(val, default(T));
        }
    }
}
