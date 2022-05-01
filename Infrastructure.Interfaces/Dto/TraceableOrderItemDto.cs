using Entities.Models.Expansion;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Interfaces.Dto
{


    public class TraceableOrderItemDto : EntityDto, IHasBasket
    {
        [Column("orderId")] // FK to Order
        public Guid OrderId { get; set; }

        //[Column("ecomItemId")]
        //public Guid? EcomItemId { get; set; } // from collectable info from ecom core

        [Column("basketId")]
        public Guid BasketId { get; set; }

        [Column("lagerId")]
        public int LagerId { get; set; }

        
        [Column("orderNumber", TypeName = "nvarchar(64)")]
        public string OrderNumber { get; set; }


        [Column("externalOrderId")]
        public int ExternalOrderId { get; set; }


        [Column("customParams", TypeName = "nvarchar(2000)")]
        public string CustomParams { get; set; }


        [Column("isActivityEnable")]
        public bool? IsActivityEnable { get; set; }


        [Column("containerBarcode", TypeName = "nvarchar(128)")]
        public string ContainerBarcode { get; set; }


        [Column("dateModified")]
        public DateTime DateModified { get; set; }


        [Column("freezeStatus", TypeName = "tinyint")]
        public int? FreezeStatus { get; set; }


        [Column("globalUserId")]
        public int? GlobalUserId { get; set; }

        [Column("pickerName", TypeName = "nvarchar(98)")]
        public string PickerName { get; set; }

        [Column("userInn", TypeName = "nvarchar(20)")]
        public string UserInn { get; set; }  // added


        [Column("lagerName", TypeName = "nvarchar(200)")]
        public string LagerName { get; set; }


        [Column("lagerUnit", TypeName = "nvarchar(100)")]
        public string LagerUnit { get; set; }


        [Column("orderQuantity", TypeName = "decimal(12,3)")]
        public decimal OrderQuantity { get; set; }


        [Column("pickerQuantity", TypeName = "decimal(12,3)")]
        public decimal? PickerQuantity { get; set; }


        [Column("priceOut", TypeName = "decimal(18,5)")]
        public decimal PriceOut { get; set; }

        [Column("replacementOnLagerId")]
        public int? ReplacementOnLagerId { get; set; }

        [Column("replacementLagers", TypeName = "nvarchar(250)")]
        public string ReplacementLagers { get; set; }

        [Column("isWeighted")]
        public bool? IsWeighted { get; set; }

        [Column("сollectable")]
        public bool Collectable { get; set; } = true;

        [Column("isFilled")]
        public bool? IsFilled { get; set; }

        [Column("rowNum")]
        public int RowNum { get; set; }

        [Column("sortingCategory", TypeName = "nvarchar(250)")]
        public string SortingCategory { get; set; }

    }
}
