using System;

namespace Maker.Identity.Stores.Entities
{
	/// <summary>
	/// Represents a login and its associated provider for a user.
	/// </summary>
	public class UserLoginBase : ISupportAssign<UserLoginBase>
	{
		/// <summary>
		/// Gets or sets the primary key of the user associated with this login.
		/// </summary>
		public virtual string UserId { get; set; }

		/// <summary>
		/// Gets or sets the login provider for the login (e.g. facebook, google)
		/// </summary>
		public virtual string LoginProvider { get; set; }

		/// <summary>
		/// Gets or sets the unique provider identifier for this login.
		/// </summary>
		public virtual string ProviderKey { get; set; }

		/// <summary>
		/// Gets or sets the friendly name used in a UI for this login.
		/// </summary>
		public virtual string ProviderDisplayName { get; set; }

		public virtual void Assign(UserLoginBase other)
		{
			UserId = other.UserId;
			LoginProvider = other.LoginProvider;
			ProviderKey = other.ProviderKey;
			ProviderDisplayName = other.ProviderDisplayName;
		}
	}

	public class UserLogin : UserLoginBase
	{
		// nothing
	}

	public class UserLoginHistory : UserLoginBase, IHistoryEntity<UserLoginBase>
	{
		/// <inheritdoc/>
		public string TransactionId { get; set; }

		/// <inheritdoc/>
		public DateTimeOffset CreatedWhen { get; set; }

		/// <inheritdoc/>
		public DateTimeOffset RetiredWhen { get; set; }
	}
}