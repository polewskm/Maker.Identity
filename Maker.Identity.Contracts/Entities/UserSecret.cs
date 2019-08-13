namespace Maker.Identity.Contracts.Entities
{
    public class UserSecret : ISupportId
    {
        public long Id { get; set; }

        public long UserId { get; set; }

        public long SecretId { get; set; }

        public User User { get; set; }

        public Secret Secret { get; set; }
    }
}