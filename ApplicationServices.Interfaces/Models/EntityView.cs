namespace ApplicationServices.Interfaces.Models
{
    /// <summary>
    /// Вид сущности
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    public abstract class EntityView<TId>
    {
        /// <summary>
        /// Идентификатор сущности
        /// </summary>
        public virtual TId Id { get; set; }
    }
}
