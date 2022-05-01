using ECom.Entities.Models;

namespace DomainServices.Interfaces.Delegates
{
    public delegate string LogisticTypeCalculator(IEnumerable<CatalogInfo> catalogInfos);
}
