using ECom.Entities.Models;
using HealthCheck.Controllers.Catalog.Requests;

namespace HealthCheck.Controllers.Catalog.Responses
{
    public class CheckCatalogResponse
    {
        public CheckCatalogRequest Request { get; set; }

        public CatalogInfo CatalogInfo { get; set; }
    }
}
