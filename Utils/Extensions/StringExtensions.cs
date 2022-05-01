using System.Text;
using System.Web;

namespace Utils.Sys.Extensions
{
    public static class StringExtensions
    {
        public static string UrlEncode(this string input, Encoding encoding = null)
        {
            return HttpUtility.UrlEncode(input, encoding ?? Encoding.UTF8)?.Replace("+", "%20");
        }
    }
}
