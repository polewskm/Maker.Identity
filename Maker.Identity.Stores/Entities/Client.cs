using System;

namespace Maker.Identity.Stores.Entities
{
    public abstract class ClientBase : ISupportAssign<ClientBase>
    {
        public long ClientId { get; set; }

        public bool Disabled { get; set; }

        public bool RequireSecret { get; set; }

        public virtual void Assign(ClientBase other)
        {
            ClientId = other.ClientId;
            Disabled = other.Disabled;
            RequireSecret = other.RequireSecret;
        }
    }

    public class Client : ClientBase, ISupportConcurrencyToken
    {
        /// <inheritdoc/>
        public string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();
    }

    public class ClientHistory : ClientBase, IHistoryEntity<ClientBase>
    {
        /// <inheritdoc/>
        public long TransactionId { get; set; }

        /// <inheritdoc/>
        public DateTime CreatedWhenUtc { get; set; }

        /// <inheritdoc/>
        public DateTime RetiredWhenUtc { get; set; }
    }
}