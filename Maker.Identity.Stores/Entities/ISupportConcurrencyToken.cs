namespace Maker.Identity.Stores.Entities
{
    public interface ISupportConcurrencyToken
    {
        /// <summary>
        /// A random value that must change whenever a user is persisted to the store
        /// </summary>
        string ConcurrencyStamp { get; set; }
    }
}