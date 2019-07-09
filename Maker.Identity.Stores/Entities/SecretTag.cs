using System;

namespace Maker.Identity.Stores.Entities
{
    public abstract class SecretTagBase : Tag<SecretTagBase>
    {
        /// <summary>
        /// Gets or sets the primary key of the secret that tag belongs to.
        /// </summary>
        public long SecretId { get; set; }

        public override void Assign(SecretTagBase other)
        {
            base.Assign(other);

            SecretId = other.SecretId;
        }
    }

    public class SecretTag : SecretTagBase, ISupportConcurrencyToken
    {
        /// <inheritdoc/>
        public string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();
    }

    public class SecretTagHistory : SecretTagBase, IHistoryEntity<SecretTagBase>
    {
        /// <inheritdoc/>
        public long TransactionId { get; set; }

        /// <inheritdoc/>
        public DateTime CreatedWhenUtc { get; set; }

        /// <inheritdoc/>
        public DateTime RetiredWhenUtc { get; set; }
    }

}