namespace Maker.Identity.Contracts.Events
{
    public class AuthEvent : EventBase
    {
        public string AuthMethod { get; set; }

        public long? UserId { get; set; }

        public long? ClientId { get; set; }
    }
}