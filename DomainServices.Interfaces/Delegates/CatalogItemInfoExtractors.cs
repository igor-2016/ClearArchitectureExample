using ECom.Entities.Models;
using ECom.Types.Delivery;
using ECom.Types.Orders;

namespace DomainServices.Interfaces.Delegates
{
    public delegate Task<CatalogInfo> CatalogSingleItemInfoExtractor(int lagerId, int filialId, Merchant merchant,
        CancellationToken cancellationToken, DeliveryType ecomDeliveryType = DeliveryType.Unknown,  Guid? basketGuid = null);
}
