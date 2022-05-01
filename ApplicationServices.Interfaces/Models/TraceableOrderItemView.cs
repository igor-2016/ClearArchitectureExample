using Entities.Models.Expansion;

namespace ApplicationServices.Interfaces.Models
{
    public class TraceableOrderItemView : EntityView<Guid>, IHasBasket
    {
        public Guid OrderId { get; set; } // FK

        //public Guid? EcomItemId { get; set; } // from collectable info from ecom core

        public Guid BasketId { get; set; }

        public CatalogItemView CatalogItem { get; set; }

        public string OrderNumber { get; set; }


        public int? ExternalOrderId { get; set; }


        public string ContainerBarcode { get; set; }


        public DateTime DateModified { get; set; }


        public PickerView Picker { get; set; }


        public decimal OrderQuantity { get; set; }


        public decimal? PickerQuantity { get; set; }


        public decimal PriceOut { get; set; }


        public int? ReplacementOnLagerId { get; set; }

        public int[] ReplacementLagers { get; set; }

    
        public bool? IsFilled { get; set; }

     
        public int RowNum { get; set; }

       
        public virtual ItemParamsView Params { get; set; }
        
    }
}
