using Utils.Sys.Options;

namespace Collecting.Fozzy.Clients.Options
{
    public class FozzyShopCollectingServiceOptions : HttpClientOptions
    {
        public string GetByOrderMethodFormat { get; set; }

        public string PutOrderMethod { get; set; }
    }
}
