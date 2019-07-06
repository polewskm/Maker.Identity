using System;

namespace Maker.Identity.Stores.Entities
{
	public abstract class RoleBase : ISupportAssign<RoleBase>
	{
		/// <summary>
		/// Gets or sets the primary key for this role.
		/// </summary>
		public virtual string RoleId { get; set; }

		/// <summary>
		/// Gets or sets the normalized name for this role.
		/// </summary>
		public virtual string NormalizedName { get; set; }

		/// <summary>
		/// Gets or sets the name for this role.
		/// </summary>
		public virtual string Name { get; set; }

		/// <inheritdoc/>
		public virtual void Assign(RoleBase other)
		{
			RoleId = other.RoleId;
			NormalizedName = other.NormalizedName;
			Name = other.Name;
		}

		/// <inheritdoc/>
		public override string ToString() => Name;
	}

	public class Role : RoleBase, ISupportConcurrencyToken
	{
		/// <inheritdoc/>
		public string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();
	}

	public class RoleHistory : RoleBase, IHistoryEntity<RoleBase>
	{
		/// <inheritdoc/>
		public string TransactionId { get; set; } = Guid.NewGuid().ToString();

		/// <inheritdoc/>
		public DateTimeOffset CreatedWhen { get; set; }

		/// <inheritdoc/>
		public DateTimeOffset RetiredWhen { get; set; }
	}
}