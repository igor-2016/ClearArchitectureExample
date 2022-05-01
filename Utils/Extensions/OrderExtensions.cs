using System.Reflection;

namespace Utils
{
    public static class OrderExtensions
    {
        public const int BasePriority = 2000000000;

        public static string ToVerString(this byte[] version)
            => version == null ? "Null" : string.Join("-", version.Select(x => string.Format("{0:x2}", x)));

        public static string BytesToString(this byte[] bytes)
            => Convert.ToBase64String(bytes);

       
        public static string ToCommaString(this IEnumerable<int> ids)
        {
            return string.Join(",", ids.EmptyIfNull2());
        }

        public static string ToCommaString(this IEnumerable<string> strs)
        {
            return string.Join(",", strs.EmptyIfNull2());
        }

        public static int ToPriority2(this DateTime date, DateTime timeFrom)
        {
            return ToPriority(new DateTime(date.Year, date.Month, date.Day, timeFrom.Hour, timeFrom.Minute, timeFrom.Second));
        }

        public static int ToPriority(this DateTime date)
        {
            var unixTime = double.Parse((date.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds.ToString());
            return (int)(BasePriority - unixTime);
        }

        public static int[] ToArrayOfInts(this string input, char separator = ',')
        {
            if (string.IsNullOrEmpty(input))
            {
                return new int[0];
            }

            string[] array = input.Split(separator, '\u0001');
            List<int> list = new List<int>();
            string[] array2 = array;
            foreach (string s in array2)
            {
                if (int.TryParse(s, out int result))
                {
                    list.Add(result);
                }
            }

            return list.ToArray();
        }
    }
}