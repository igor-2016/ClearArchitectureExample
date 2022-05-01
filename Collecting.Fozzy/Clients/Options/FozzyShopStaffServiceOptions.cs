using Utils.Sys.Options;

namespace Collecting.Fozzy.Clients.Options
{
    public class FozzyShopStaffServiceOptions : HttpClientOptions
    {
        public string GetByGlobalIdMethodFormat { get; set; }

        public string GetByInnMethodFormat { get; set; }
    }
}
