using Catalog.Interfaces.Enums;

namespace Catalog.Interfaces.Dto
{
    public class CatalogResponse
    {
        public CatalogGoodsDataRequestMode Mode { get; set; } 

        public CatalogGoodsData GoodsData { get; set; }

    }
}
