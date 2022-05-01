using Utils.Sys.Options;

namespace Catalog.Ecom.Options
{
    public class CatalogApiOptions : HttpClientOptions
    {
        public string GetItemsMethod { get; set; }
    }
}
