using System;
using Maker.Identity.Contracts.Entities;

namespace Maker.Identity.Contracts.Events
{
    public class EventBase : ISupportId
    {
        public long Id { get; set; }

        public DateTime TimestampUtc { get; set; }

        public EventTypes EventType { get; set; }

        public int EventId { get; set; }

        public string ActivityId { get; set; }

        public string CorrelationId { get; set; }
    }
}