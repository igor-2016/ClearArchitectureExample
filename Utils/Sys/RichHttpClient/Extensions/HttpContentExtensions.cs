using System.Text;
using Utils.Sys.Extensions;

namespace Utils.Sys.RichHttpClient.Extensions
{
    public static class HttpContentExtensions
    { 
        public static string ReadAsString(this HttpContent content)
        {
            return content.ReadAsStringAsync().GetAwaiter().GetResult();
        }

        public static string FromUrlEncodeContent(this IDictionary<string, string> nameValueCollection)
        {
            string Encode(string data)
            {
                return Uri.EscapeDataString(data).Replace("%20", "+", StringComparison.InvariantCultureIgnoreCase);
            }

            var stringBuilder = new StringBuilder();
            foreach (var nameValue in nameValueCollection)
            {
                if (stringBuilder.Length > 0)
                {
                    stringBuilder.Append('&');
                }

                stringBuilder.Append(Encode(nameValue.Key));
                stringBuilder.Append('=');
                stringBuilder.Append(Encode(nameValue.Value));
            }

            return stringBuilder.ToString();
        }

        public static string FromUrlEncodeContent(this object obj)
            => obj.AsDictionary().FromUrlEncodeContent();
    }

}
