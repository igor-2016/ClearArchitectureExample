using System;
using System.Collections.Generic;
using System.Linq;

namespace Utils.Sys.Extensions
{
    public static class ArgumentExtensions
    {
        public static IEnumerable<TSource> CheckEmpty<TSource>(
            this IEnumerable<TSource> source,
            string argumentName)
            where TSource : class
        {
            if (source == null)
            {
                throw new ArgumentNullException(argumentName.CheckNull(nameof(argumentName)));
            }

            if (!source.Any())
            {
                throw new ArgumentOutOfRangeException(argumentName.CheckNull(nameof(argumentName)));
            }

            return source;
        }

        public static TSource CheckNull<TSource>(
            this TSource source,
            string argumentName)
            where TSource : class
        {
            return source ?? throw new ArgumentNullException(argumentName.CheckNull(nameof(argumentName)));
        }

        public static TSource DefaultIfNull<TSource>(
            this TSource? source,
            TSource defaultValue)
            where TSource : class
        {
            return source ?? defaultValue.CheckNull(nameof(defaultValue));
        }
    }

}
