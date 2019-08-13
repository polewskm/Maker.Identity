namespace Maker.Identity.Contracts.Entities
{
    public class RoleClaim : ClaimBase
    {
        /// <summary>
        /// Gets or sets the of the primary key of the role associated with this claim.
        /// </summary>
        public long RoleId { get; set; }

        public Role Role { get; set; }
    }
}