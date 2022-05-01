using ECom.Types.Orders;

namespace DomainServices.Interfaces.Delegates
{
    public delegate int? OrderOriginExtractor(int merchantId, int filialId, BasketType basketType);

    //public delegate int? OrderOriginExtractorByBasketId(Guid basketId);
}
