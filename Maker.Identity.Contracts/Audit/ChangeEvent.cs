using System;
using Maker.Identity.Contracts.Entities;

namespace Maker.Identity.Contracts.Audit
{
    public class ChangeEvent : ISupportId
    {
        public long Id { get; set; }

        public DateTime TimestampUtc { get; set; }

        public ChangeType ChangeType { get; set; }

        public string UserName { get; set; }

        public string PrincipalName { get; set; }

        public byte[] PrincipalKey { get; set; }

        public string KeyValues { get; set; }

        public string OldValues { get; set; }

        public string NewValues { get; set; }
    }
}