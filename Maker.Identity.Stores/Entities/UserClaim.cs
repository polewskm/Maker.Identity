using System;
using System.Security.Claims;

namespace Maker.Identity.Stores.Entities
{
    /// <summary>
    /// Represents a claim that a user possesses.
    /// </summary>
    public class UserClaimBase : ISupportAssign<UserClaimBase>
    {
        /// <summary>
        /// Gets or sets the identifier for this user claim.
        /// </summary>
        public long UserClaimId { get; set; }

        /// <summary>
        /// Gets or sets the primary key of the user associated with this claim.
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// Gets or sets the claim type for this claim.
        /// </summary>
        public string ClaimType { get; set; }

        /// <summary>
        /// Gets or sets the claim value for this claim.
        /// </summary>
        public string ClaimValue { get; set; }

        public virtual void Assign(UserClaimBase other)
        {
            UserClaimId = other.UserClaimId;
            UserId = other.UserId;
            ClaimType = other.ClaimType;
            ClaimValue = other.ClaimValue;
        }

        /// <summary>
        /// Converts the entity into a Claim instance.
        /// </summary>
        public virtual Claim ToClaim()
        {
            return new Claim(ClaimType, ClaimValue);
        }

        /// <summary>
        /// Reads the type and value from the Claim.
        /// </summary>
        public virtual void InitializeFromClaim(Claim claim)
        {
            ClaimType = claim.Type;
            ClaimValue = claim.Value;
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