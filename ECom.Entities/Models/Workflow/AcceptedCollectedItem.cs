using ECom.Types.Orders;

namespace ECom.Entities.Models
{

    /// <summary>
    /// старая посылка на ECom.Core через шину и новая через ECom.Workflow
    /// </summary>
    public class AcceptedCollectedItem : BasketCollectedItem
    {
        /// <summary>
        /// Не хватает этого поля, убрать после обновления новых типов 
        /// </summary>
        public decimal? OrderQty { get; set; }
    }
}
