namespace Maker.Identity.Contracts.Entities
{
    /// <summary>
    /// Represents a claim that a user possesses.
    /// </summary>
    public class UserClaim : ClaimBase
    {
        /// <summary>
        /// Gets or sets the primary key of the user associated with this claim.
        /// </summary>
        public long UserId { get; set; }

        public User User { get; set; }
    }
}