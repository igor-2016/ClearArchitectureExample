namespace Entities.Models.Expansion
{
    public abstract class VersionedEntity<TId> : Entity<TId>
    {
        public string RowVersion { get; set; }
    }
}
