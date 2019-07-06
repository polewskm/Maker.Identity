using System;

namespace Maker.Identity.Stores.Entities
{
	public abstract class ClientBase : ISupportAssign<ClientBase>
	{
		/// <summary>
		/// Gets or sets the primary key for this instance.
		/// </summary>
		public string ClientId { get; set; } = Guid.NewGuid().ToString();

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
		public string TransactionId { get; set; } = Guid.NewGuid().ToString();

		/// <inheritdoc/>
		public DateTimeOffset CreatedWhen { get; set; }

		/// <inheritdoc/>
		public DateTimeOffset RetiredWhen { get; set; }
	}
}