namespace Entities.Models.Expansion
{
    /// <summary>
    /// Сущность
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    public abstract class Entity<TId>
    {
        /// <summary>
        /// Идентификатор сущности
        /// </summary>
        public virtual TId Id { get; set; }
    }
}
