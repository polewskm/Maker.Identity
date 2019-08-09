namespace Maker.Identity.Stores.Entities
{
    public abstract class ClaimBase<T> : ISupportAssign<T>
        where T : ClaimBase<T>
    {
        /// <summary>
        /// Gets or sets the identifier for this claim.
        /// </summary>
        public long ClaimId { get; set; }

        /// <summary>
        /// Gets or sets the claim type for this claim.
        /// </summary>
        public string ClaimType { get; set; }

        /// <summary>
        /// Gets or sets the claim value for this claim.
        /// </summary>
        public string ClaimValue { get; set; }

        /// <inheritdoc/>
        public virtual void Assign(T other)
        {
            ClaimId = other.ClaimId;
            ClaimType = other.ClaimType;
            ClaimValue = other.ClaimValue;
        }

    }
}