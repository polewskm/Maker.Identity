using System;

namespace Maker.Identity.Contracts.Entities
{
    public class AuditChange : ISupportId
    {
        public long Id { get; set; }

        public DateTime TimestampUtc { get; set; }

        public AuditChangeType ChangeType { get; set; }

        public string PrincipalName { get; set; }

        public byte[] PrincipalKey { get; set; }

        public string KeyValues { get; set; }

        public string OldValues { get; set; }

        public string NewValues { get; set; }
    }
}