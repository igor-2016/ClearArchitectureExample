namespace ECom.Expansion.TestHelpers
{
    public static class HttpClientHelper
    {
        public static HttpClient GetHttpClient(string baseUrl, TimeSpan timeout)
        {
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(baseUrl);
            httpClient.Timeout = timeout;
            return httpClient;
        }

        public static HttpClient GetHttpClient(string baseUrl)
        {
            return GetHttpClient(baseUrl, TimeSpan.FromMinutes(10));
        }
    }
}
