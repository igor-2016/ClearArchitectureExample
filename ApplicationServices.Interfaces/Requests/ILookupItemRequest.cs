using Entities.Models;

namespace ApplicationServices.Interfaces.Requests
{
    public interface ILookupItemRequest : IQuery
    {
        //
        // Summary:
        //     Идентификатор позиции заказа
        Guid? ItemId { get; set; }
        //
        // Summary:
        //     Штрихкод
        string Barcode { get; set; }
        //
        // Summary:
        //     Артикул
        int? LagerId { get; set; }
    }
}
