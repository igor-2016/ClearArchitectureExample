namespace Entities.Models.Workflow
{

    /// <summary>
    /// Пока не используется
    /// </summary>
    public class NewAcceptedCollectedItem
    {
        //для заполнения FozzyOrderLine

        public Guid Id { get; set; } // line id  (lager is not unique)


        public int LagerId { get; set; }


        public string OrderNumber { get; set; }  // строчный ордер 


        public int? ExternalOrderId { get; set; } // в ТСД Чернова


        //public string CustomParams { get; set; }  // TODO заполнить для существующего и новых позиций


        public bool? IsActivityEnable { get; set; }


        public string ContainerBarcode { get; set; }


        public DateTime DateModified { get; set; }


        public int? FreezeStatus { get; set; }


        public int? GlobalUserId { get; set; }  // пикер

        public string PickerName { get; set; }

        //public string UserInn { get; set; }  // TODO не забыть взять из существующего в Expansion


        public string LagerName { get; set; }


        public string LagerUnit { get; set; }


        public decimal OrderQuantity { get; set; }


        public decimal? PickerQuantity { get; set; }


        public decimal PriceOut { get; set; }


        public int? ReplacementOnLagerId { get; set; }

        public string ReplacementLagers { get; set; }

        public bool? IsWeighted { get; set; }

        public bool? IsFilled { get; set; }

        public bool Collectable { get; set; } = true;

        public int RowNum { get; set; } // возможно будет для сортировки


        // старая посылка на ECom.Core через шину

        /*
         
        public int LagerId
        {
            get;
            set;
        }

         public decimal OrderQty
        {
            get;
            set;
        }

        public decimal PickerQty
        {
            get;
            set;
        }

        public int? PickerId
        {
            get;
            set;
        }

        public string PickerName
        {
            get;
            set;
        }

        public bool Collectable
        {
            get;
            set;
        }

        public IEnumerable<string> ExciseBarcodes
        {
            get;
            set;
        }

        public int? ReplacementOnLagerId
        {
            get;
            set;
        }

        public int[] Replacements
        {
            get;
            set;
        } = new int[0];
        */
    }
}
