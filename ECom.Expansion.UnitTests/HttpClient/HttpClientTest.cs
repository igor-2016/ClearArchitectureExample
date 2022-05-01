using ECom.Expansion.TestHelpers;
using System;
using Xunit;

namespace ECom.Expansion.UnitTests.HttpClient
{
    public class HttpClientTest
    {
        [Fact]
        public void CreateHttpClient()
        {
            var baseUrl = "https://docs.microsoft.com/";
            var httpClient = HttpClientHelper.GetHttpClient(baseUrl);
            Assert.IsType<System.Net.Http.HttpClient>(httpClient);
            Assert.Equal(baseUrl, httpClient.BaseAddress.ToString());
            Assert.Equal(TimeSpan.FromSeconds(100), httpClient.Timeout);
        }

        [Fact]
        public void CreateHttpWithTimeoutClient()
        {
            var timeout = TimeSpan.FromSeconds(200);
            var baseUrl = "https://docs.microsoft.com/";
            var httpClient = HttpClientHelper.GetHttpClient(baseUrl, timeout);
            Assert.IsType<System.Net.Http.HttpClient>(httpClient);
            Assert.Equal(baseUrl, httpClient.BaseAddress.ToString());
            Assert.Equal(timeout, httpClient.Timeout);
        }
    }
}
