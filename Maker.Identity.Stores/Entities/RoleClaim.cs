using System;
using System.Security.Claims;

namespace Maker.Identity.Stores.Entities
{
	public abstract class RoleClaimBase : ISupportAssign<RoleClaimBase>
	{
		/// <summary>
		/// Gets or sets the identifier for this role claim.
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// Gets or sets the of the primary key of the role associated with this claim.
		/// </summary>
		public string RoleId { get; set; }

		/// <summary>
		/// Gets or sets the claim type for this claim.
		/// </summary>
		public string ClaimType { get; set; }

		/// <summary>
		/// Gets or sets the claim value for this claim.
		/// </summary>
		public string ClaimValue { get; set; }

		public virtual void Assign(RoleClaimBase other)
		{
			Id = other.Id;
			RoleId = other.RoleId;
			ClaimType = other.ClaimType;
			ClaimValue = other.ClaimValue;
		}

		/// <summary>
		/// Constructs a new claim with the type and value.
		/// </summary>
		public virtual Claim ToClaim()
		{
			return new Claim(ClaimType, ClaimValue);
		}

		/// <summary>
		/// Initializes by copying ClaimType and ClaimValue from the other claim.
		/// </summary>
		/// <param name="other">The claim to initialize from.</param>
		public virtual void InitializeFromClaim(Claim other)
		{
			ClaimType = other?.Type;
			ClaimValue = other?.Value;
		}
	}

	public class RoleClaim : RoleClaimBase
	{
		// nothing
	}

	public class RoleClaimHistory : RoleClaimBase, IHistoryEntity<RoleClaimBase>
	{
		/// <inheritdoc/>
		public string TransactionId { get; set; } = Guid.NewGuid().ToString();

		/// <inheritdoc/>
		public DateTimeOffset CreatedWhen { get; set; }

		/// <inheritdoc/>
		public DateTimeOffset RetiredWhen { get; set; }
	}
}