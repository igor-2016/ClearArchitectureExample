namespace ECom.Entities.Models
{
    /// <summary>
    /// Информация о товаре из каталога
    /// </summary>
    public class CatalogItem
    {
        /// <summary>
        /// LagerId товара
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Наименование товара
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Единица измерения
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// Наименование товара для отображения на сайте
        /// </summary>
        public string NameForSite { get; set; }

        public decimal? PriceOld { get; set; }

        public decimal Price { get; set; }

        public decimal? PriceOpt { get; set; }

        /// <summary>
        /// Признак сухой, заморозка, охлождёнка (наименование)
        /// </summary>
        public string FreezeStatus { get; set; }

        /// <summary>
        /// Признак сухой, заморозка, охлождёнка... (код)
        /// </summary>
        public int? FreezeStatusId { get; set; }

        /// <summary>
        /// Товар весовой
        /// </summary>
        public bool IsWeighted { get; set; }

        /// <summary>
        /// Признак акционного товара
        /// </summary>
        public bool? IsActivityEnable { get; set; }


        /// <summary>
        /// Может быть собран?
        /// </summary>
        public bool? IsCollectable { get; set; }  
        

        /// <summary>
        /// Категория для формирования RowNum присборке
        /// </summary>
        public string SortingCategory { get; set; }


        public CatalogItemPrices GetPrices()
        {
            return new CatalogItemPrices()
            {
                Price = Price,
                PriceOld = PriceOld,
                PriceOpt = PriceOpt,
            };
        }
    }
}

