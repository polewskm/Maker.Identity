using System;

namespace Maker.Identity.Stores.Entities
{
	public abstract class SecretBase : ISupportAssign<SecretBase>
	{
		/// <summary>
		/// Gets or sets the primary key for this instance.
		/// </summary>
		public string SecretId { get; set; } = Guid.NewGuid().ToString();

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
		public string TransactionId { get; set; } = Guid.NewGuid().ToString();

		/// <inheritdoc/>
		public DateTimeOffset CreatedWhen { get; set; }

		/// <inheritdoc/>
		public DateTimeOffset RetiredWhen { get; set; }
	}

}