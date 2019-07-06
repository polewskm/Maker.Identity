using System;

namespace Maker.Identity.Stores.Entities
{
	/// <summary>
	/// Represents the link between a user and a role.
	/// </summary>
	public class UserRoleBase : ISupportAssign<UserRoleBase>
	{
		/// <summary>
		/// Gets or sets the primary key of the user that is linked to a role.
		/// </summary>
		public virtual string UserId { get; set; }

		/// <summary>
		/// Gets or sets the primary key of the role that is linked to the user.
		/// </summary>
		public virtual string RoleId { get; set; }

		/// <inheritdoc/>
		public virtual void Assign(UserRoleBase other)
		{
			UserId = other.UserId;
			RoleId = other.RoleId;
		}
	}

	public class UserRole : UserRoleBase
	{
		// nothing
	}

	public class UserRoleHistory : UserRoleBase, IHistoryEntity<UserRoleBase>
	{
		/// <inheritdoc/>
		public string TransactionId { get; set; }

		/// <inheritdoc/>
		public DateTimeOffset CreatedWhen { get; set; }

		/// <inheritdoc/>
		public DateTimeOffset RetiredWhen { get; set; }
	}
}