using System;

namespace Maker.Identity.Stores.Entities
{
    public abstract class RoleClaimBase : ClaimBase<RoleClaimBase>
    {
        /// <summary>
        /// Gets or sets the of the primary key of the role associated with this claim.
        /// </summary>
        public long RoleId { get; set; }

        /// <inheritdoc/>
        public override void Assign(RoleClaimBase other)
        {
            base.Assign(other);

            RoleId = other.RoleId;
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