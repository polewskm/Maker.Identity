namespace Maker.Identity.Contracts.Entities
{
    public class SecretTag : TagBase
    {
        /// <summary>
        /// Gets or sets the primary key of the secret that tag belongs to.
        /// </summary>
        public long SecretId { get; set; }

        public Secret Secret { get; set; }
    }
}