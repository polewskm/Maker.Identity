namespace Maker.Identity.Contracts.Audit
{
    public class ChangeEvent : EventBase
    {
        public string UserName { get; set; }

        public string PrincipalName { get; set; }

        public byte[] PrincipalKey { get; set; }

        public string KeyValues { get; set; }

        public string OldValues { get; set; }

        public string NewValues { get; set; }
    }
}