using Utils.Sys.Options;

namespace WebSite.Presta.Clients.Options
{
    public class FozzyShopSiteOptions : HttpClientOptions
    {
        public string Login { get; set; }

        public string Password { get; set; }

        public string PutOrderMethod { get; set; }
    }
}
