using System;

namespace Maker.Identity.Stores.Entities
{
	public abstract class SecretTagBase : Tag<SecretTagBase>
	{
		/// <summary>
		/// Gets or sets the primary key of the secret that tag belongs to.
		/// </summary>
		public string SecretId { get; set; }

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
		public string TransactionId { get; set; } = Guid.NewGuid().ToString();

		/// <inheritdoc/>
		public DateTimeOffset CreatedWhen { get; set; }

		/// <inheritdoc/>
		public DateTimeOffset RetiredWhen { get; set; }
	}

}