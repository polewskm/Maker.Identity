using System;

namespace Maker.Identity.Stores.Entities
{
    public abstract class SecretBase : ISupportAssign<SecretBase>
    {
        /// <summary>
        /// Gets or sets the primary key for this instance.
        /// </summary>
        public long SecretId { get; set; }

        public string CipherType { get; set; }

        public string CipherText { get; set; }

        public virtual void Assign(SecretBase other)
        {
            SecretId = other.SecretId;
            CipherType = other.CipherType;
            CipherText = other.CipherText;
        }
    }

    public class Secret : SecretBase, ISupportConcurrencyToken
    {
        /// <inheritdoc/>
        public string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();
    }

    public class SecretHistory : SecretBase, IHistoryEntity<SecretBase>
    {
        /// <inheritdoc/>
        public long TransactionId { get; set; }

        /// <inheritdoc/>
        public DateTime CreatedWhenUtc { get; set; }

        /// <inheritdoc/>
        public DateTime RetiredWhenUtc { get; set; }
    }

}