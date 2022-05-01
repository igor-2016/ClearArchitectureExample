using Newtonsoft.Json;

namespace Utils.Sys.Extensions
{
    public static class JsonSerializationExtensions
    {
        public static string SerializeJson(this object o, Formatting formatting = Formatting.Indented)
        {
            return JsonConvert.SerializeObject(o, formatting);
        }

        public static T DeserializeJson<T>(this string s)
            where T : class
        {
            return !string.IsNullOrWhiteSpace(s) ? JsonConvert.DeserializeObject<T>(s) : null;
        }

        public static IDictionary<string, string> AsDictionary(this object obj)
        {
            return obj?.SerializeJson().DeserializeJson<Dictionary<string, string>>() ?? new Dictionary<string, string>();
        }
    }
}
