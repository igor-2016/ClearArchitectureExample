using Entities.Models.Collecting;
using System.Globalization;

namespace DomainServices.Interfaces
{
    public static class OrderDataMappingExtension
    {
        
        private readonly static IFormatProvider _formater = new CultureInfo("en");
   
        public static string ToDecimalString(this decimal input)
        {
            return input.ToString(_formater); 
        }
        public static string ToDecimalString(this decimal? input)
        {
            return input.HasValue ? input.Value.ToDecimalString() : "0";
        }

        public static decimal ToDecimal(this string input)
        {
            return decimal.Parse(input.Replace(',', '.'), _formater); 
        }

        public static decimal? ToDecimalNull(this string input)
        {
            if(string.IsNullOrEmpty(input))
                return null;

            return decimal.Parse(input.Replace(',', '.'), _formater);
        }

        public static DateTime? ToDateTime(this string sqlDateTimeString)
        {
            if(string.IsNullOrEmpty(sqlDateTimeString))
            {
                return null;
            }

            DateTime dateTime;
            if (!DateTime.TryParseExact(sqlDateTimeString,
                new[] { "yyyy-MM-dd HH:mm:ss", "dd.MM.yyyy HH:mm:ss", "dd.MM.yyyy H:mm:ss" }, null, DateTimeStyles.None, out dateTime))
            {
                return null;
            }
            return dateTime;
        }

        public static DateTime? ToDateOnly(this string sqlDateTimeString)
        {
            if (string.IsNullOrEmpty(sqlDateTimeString))
            {
                return null;
            }

            DateTime dateTime;
            if (!DateTime.TryParseExact(sqlDateTimeString,
                new[] { "yyyy-MM-dd", "dd.MM.yyyy", "dd.MM.yyyy" }, null, DateTimeStyles.None, out dateTime))
            {
                return null;
            }
            return dateTime;
        }


        public static bool ToBool(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return false;

            bool result;
            input = input
                .Replace("1", "True")
                .Replace("0", "False")
                ;
            bool.TryParse(input, out result);
            return result;
        }

        public static int? ToIntNulable(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return null;

            if (int.TryParse(input, out int intValue))
                return intValue;

            return null;
        }

        public static int ToInt(this string input)
        {
            return int.Parse(input, _formater); 
        }

        public static int? NullFromIntZero(this int input)
        {
            return input == 0 ? null : input;
        }

        public static int? ToIntNull(this string input)
        {
            int val;
            if (int.TryParse(input, out val))
                return val;
            return null;
        }

        public static int ToInt(this Enum en)
        {
            return (int)(object)en;
        }

       
        public static FzShopOrder GetOrder(this OrderData data)
        {
            if (!data.IsValid())
                throw new Exception("Order is invalid");

            return data.order[0];
        }

        public static FzShopOrder SetFilial(this OrderData data, int filialId)
        {
            if (!data.IsValid())
                throw new Exception("Order is invalid");

            data.order[0].filialId = filialId;

            return data.order[0];
        }

        public static byte ToByte(this Enum en)
        {
            return (byte)(object)en;
        }

        public static string ToIntString(this bool input)
        {
            return input ? "1" : "0";
        }

        public static string ToIntStringNull(this bool? input)
        {
            return input.HasValue ? ToIntString(input.Value) : "0";
        }

        public static string ToStringDateTime(this DateTimeOffset input)
        {
            return input.ToString("dd.MM.yyyy HH:mm:ss");
        }

        public static string ToStringDateTime(this DateTime input)
        {
            return input.ToString("dd.MM.yyyy HH:mm:ss");
        }

        public static string ToStringDateTimeNullable(this DateTime? input)
        {
            return input.HasValue ? input.Value.ToString("dd.MM.yyyy HH:mm:ss") : "";
        }

        public static string ToStringDate(this DateTime input)
        {
            return input.ToString("dd.MM.yyyy");
        }

        public static DateTime ToDate(this DateTime input)
        {
            return input.Date;
        }

        public static string ToStringTime(this DateTime input)
        {
            return input.ToString(@"hh\:mm\:ss");// "HH:mm:ss");
        }

        public static string ToStringTime(this TimeSpan input)
        {
            return input.ToString(@"hh\:mm\:ss");
        }

        public static DateTime ToDateTimeSlotFrom(this string input, DateTime? baseDateTime = null) // after ToStringTime
        {
            var dateNow = baseDateTime ?? DateTime.UnixEpoch;
            if(TimeSpan.TryParseExact(input, @"hh\:mm\:ss", null, out TimeSpan val))
            {
                return new DateTime(dateNow.Year, dateNow.Month, dateNow.Day, val.Hours, val.Minutes, val.Seconds);
            }
            return dateNow;
        }

        public static TimeSpan? ToTime(this string input)
        {
            if(string.IsNullOrEmpty(input))
            {
                return null;
            }

            if (TimeSpan.TryParseExact(input, @"hh\:mm\:ss", null, out TimeSpan val))
            {
                return val;
            }

            return null;
        }


        //public static string ToCommaString(this IEnumerable<CollectItemReplacementView> replacements)
        //{
        //    return string.Join(",", replacements.EmptyIfNull().Select(x => x.LagerId));
        //}



        public static string ToVerLineString(this IEnumerable<string> strs)
        {
            if(strs == null)
                return string.Empty;

            return string.Join(" | ", strs);
        }

        public static Guid ToLineId(this FzShopOrderLines line)
        {
            var value = line.GetLineId();
            return value.HasValue ? value.Value : Guid.Empty;//Guid.Parse(line.CustomParams);
        }


        public static DateTime UnixTimeStampToDateTime(this double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dateTime;
        }

      
        //public static bool IsCreatedInFozzyShop(this OrderData order)
        //{
        //    return order.order.Length > 0 ? order.order[0].orderFrom == FozzyShopOrderOrigin.Site.ToByte().ToString() : false;
        //}

        public static Guid GetBasketId(this OrderData order)
        {
            //{ "basketGuid":"5dbd4038-806d-4673-a132-46ae8fef98c9","lineId":"446436"}
            if (order.IsValid())
            {
                var basketId = order.orderLines[0].GetBasketId();

                if(!basketId.HasValue)
                    throw new Exception("Basket id not set!");
            }
            throw new Exception("Basket id not found!");
        }

        public static bool IsValid(this OrderData order)
        {
            if (order == null)
                return false;

            if (order.order == null)
                return false;

            if (order.order.Length != 1)
                return false;

            if (order.orderLines == null)
                return false;

            if (order.orderLines.Length == 0)
                return false;

            return true;
        }


    }
}
