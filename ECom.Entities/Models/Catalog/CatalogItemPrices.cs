using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECom.Entities.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class CatalogItemPrices
    {
        /// <summary>
        /// Текущая цена розницы
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Текущая оптовая цена 
        /// </summary>
        public decimal? PriceOpt { get; set; }

        /// <summary>
        /// Старая цена (опт или роз?)
        /// </summary>
        public decimal? PriceOld { get; set; }

    }
}
