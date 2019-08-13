namespace Maker.Identity.Contracts.Entities
{
    public class ClientSecret : ISupportId
    {
        public long Id { get; set; }

        public long ClientId { get; set; }

        public long SecretId { get; set; }

        public Client Client { get; set; }

        public Secret Secret { get; set; }
    }
}