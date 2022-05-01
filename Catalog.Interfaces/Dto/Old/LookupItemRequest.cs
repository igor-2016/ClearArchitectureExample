namespace Catalog.Interfaces.Dto
{
    public class LookupItemRequest 
    {
        public int DeliveryTypeId { get; set; }

        public Guid BasketGuid { get; set; }
        
        public int FilialId { get; set; }
        
        //
        // Summary:
        //     Идентификатор позиции заказа
        public Guid? ItemId
        {
            get;
            set;
        }

        //
        // Summary:
        //     Штрихкод
        public string Barcode
        {
            get;
            set;
        }

        //
        // Summary:
        //     Артикул
        public int? LagerId
        {
            get;
            set;
        }
    }
}
