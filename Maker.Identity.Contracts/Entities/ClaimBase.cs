namespace Maker.Identity.Contracts.Entities
{
    public abstract class ClaimBase : ISupportId
    {
        /// <summary>
        /// Gets or sets the identifier for this claim.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the claim type for this claim.
        /// </summary>
        public string ClaimType { get; set; }

        /// <summary>
        /// Gets or sets the claim value for this claim.
        /// </summary>
        public string ClaimValue { get; set; }
    }
}