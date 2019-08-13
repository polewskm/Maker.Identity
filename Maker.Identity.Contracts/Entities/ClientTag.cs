namespace Maker.Identity.Contracts.Entities
{
    public class ClientTag : TagBase
    {
        /// <summary>
        /// Gets or sets the primary key of the client that tag belongs to.
        /// </summary>
        public long ClientId { get; set; }

        public Client Client { get; set; }
    }
}