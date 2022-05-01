using System.Net;

namespace Utils.Attributes
{
    public static class ErrorEnumExtenstion
    {
        public static string GetResponseMessageFromEnum<TEnum>(this TEnum item)
        {
            var desc = item.GetType()
               .GetField(item.ToString())
               .GetCustomAttributes(typeof(ResponseErrorAttribute), false)
               .Cast<ResponseErrorAttribute>()
               .FirstOrDefault()?.Message ?? string.Empty;

            return desc;
        }

        public static HttpStatusCode GetHttpStatusFromEnum<TEnum>(this TEnum item)
        {
            var desc = item.GetType()
               .GetField(item.ToString())
               .GetCustomAttributes(typeof(ResponseErrorAttribute), false)
               .Cast<ResponseErrorAttribute>()
               .FirstOrDefault()?.HttpStatus ?? HttpStatusCode.BadRequest;

            return desc;
        }
    }
}
