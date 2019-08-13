using System;
using Maker.Identity.Contracts.Entities;

namespace Maker.Identity.Contracts.Audit
{
    public class UserEvent : ISupportId
    {
        public long Id { get; set; }

        public DateTime TimestampUtc { get; set; }

        public EventTypes EventType { get; set; }

        public int EventId { get; set; }

        public long UserId { get; set; }

        public long? ClientId { get; set; }

        public string ActivityId { get; set; }

        public string AuthenticationMethod { get; set; }
    }
}