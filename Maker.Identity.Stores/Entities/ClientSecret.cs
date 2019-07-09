using System;

namespace Maker.Identity.Stores.Entities
{
    public class ClientSecretBase : ISupportAssign<ClientSecretBase>
    {
        public long ClientId { get; set; }

        public long SecretId { get; set; }

        public virtual void Assign(ClientSecretBase other)
        {
            ClientId = other.ClientId;
            SecretId = other.SecretId;
        }
    }

    public class ClientSecret : ClientSecretBase
    {
        // nothing
    }

    public class ClientSecretHistory : ClientSecretBase, IHistoryEntity<ClientSecretBase>
    {
        /// <inheritdoc/>
        public long TransactionId { get; set; }

        /// <inheritdoc/>
        public DateTime CreatedWhenUtc { get; set; }

        /// <inheritdoc/>
        public DateTime RetiredWhenUtc { get; set; }
    }
}