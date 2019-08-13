namespace Maker.Identity.Contracts.Entities
{
    public interface ISupportConcurrencyToken
    {
        /// <summary>
        /// A random value that must change whenever an entity is persisted to the store
        /// </summary>
        string ConcurrencyStamp { get; set; }
    }
}