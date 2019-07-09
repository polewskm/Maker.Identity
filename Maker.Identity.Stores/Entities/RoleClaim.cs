using System;

namespace Maker.Identity.Stores.Entities
{
    public abstract class RoleClaimBase : ISupportAssign<RoleClaimBase>
    {
        /// <summary>
        /// Gets or sets the identifier for this role claim.
        /// </summary>
        public long RoleClaimId { get; set; }

        /// <summary>
        /// Gets or sets the of the primary key of the role associated with this claim.
        /// </summary>
        public long RoleId { get; set; }

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
            RoleClaimId = other.RoleClaimId;
            RoleId = other.RoleId;
            ClaimType = other.ClaimType;
            ClaimValue = other.ClaimValue;
        }
    }

    public class RoleClaim : RoleClaimBase
    {
        // nothing
    }

    public class RoleClaimHistory : RoleClaimBase, IHistoryEntity<RoleClaimBase>
    {
        /// <inheritdoc/>
        public long TransactionId { get; set; }

        /// <inheritdoc/>
        public DateTime CreatedWhenUtc { get; set; }

        /// <inheritdoc/>
        public DateTime RetiredWhenUtc { get; set; }
    }
}