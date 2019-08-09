using System;

namespace Maker.Identity.Stores.Entities
{
    /// <summary>
    /// Represents a claim that a user possesses.
    /// </summary>
    public class UserClaimBase : ClaimBase<UserClaimBase>
    {
        /// <summary>
        /// Gets or sets the primary key of the user associated with this claim.
        /// </summary>
        public long UserId { get; set; }

        /// <inheritdoc/>
        public override void Assign(UserClaimBase other)
        {
            base.Assign(other);

            UserId = other.UserId;
        }
    }

    public class UserClaim : UserClaimBase
    {
        // nothing
    }

    public class UserClaimHistory : UserClaimBase, IHistoryEntity<UserClaimBase>
    {
        /// <inheritdoc/>
        public long TransactionId { get; set; }

        /// <inheritdoc/>
        public DateTime CreatedWhenUtc { get; set; }

        /// <inheritdoc/>
        public DateTime RetiredWhenUtc { get; set; }
    }
}