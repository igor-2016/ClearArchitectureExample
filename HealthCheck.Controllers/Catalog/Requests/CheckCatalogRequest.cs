using ECom.Types.Delivery;
using ECom.Types.Orders;
using Expansion.Interfaces.Enums;
using Swashbuckle.AspNetCore.Filters;

namespace HealthCheck.Controllers.Catalog.Requests
{
    public class CheckCatalogRequest : IExamplesProvider<CheckCatalogRequest>
    {
        public static CheckCatalogRequest Default => new CheckCatalogRequest();
        public int LagerId { get; set; } = (int)Items.CocaCola;
        public int Filial { get; set; } = (int)Filials.Filial_51;
        public Merchant Merchant { get; set; } = Merchant.Fozzy;
        public DeliveryType EComDeliveryType { get; set; } = DeliveryType.Unknown;

        public CheckCatalogRequest GetExamples()
        {
            return Default;
        }

        public bool isValid()
        {
            return LagerId > 0 && Filial > 0 && (int)EComDeliveryType == 0 && (int)Merchant > 0;
        }
    }
}
