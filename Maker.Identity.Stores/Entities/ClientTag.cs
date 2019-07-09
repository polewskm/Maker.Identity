using System;

namespace Maker.Identity.Stores.Entities
{
    public abstract class ClientTagBase : Tag<ClientTagBase>
    {
        /// <summary>
        /// Gets or sets the primary key of the client that tag belongs to.
        /// </summary>
        public long ClientId { get; set; }

        public override void Assign(ClientTagBase other)
        {
            base.Assign(other);

            ClientId = other.ClientId;
        }
    }

    public class ClientTag : ClientTagBase, ISupportConcurrencyToken
    {
        /// <inheritdoc/>
        public string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();
    }

    public class ClientTagHistory : ClientTagBase, IHistoryEntity<ClientTagBase>
    {
        /// <inheritdoc/>
        public long TransactionId { get; set; }

        /// <inheritdoc/>
        public DateTimeOffset CreatedWhen { get; set; }

        /// <inheritdoc/>
        public DateTimeOffset RetiredWhen { get; set; }
    }

}