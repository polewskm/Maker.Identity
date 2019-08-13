using System;

namespace Maker.Identity.Contracts.Entities
{
    public class Client : ISupportConcurrencyToken, ISupportId
    {
        /// <inheritdoc/>
        public string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();

        public long Id { get; set; }

        public bool Disabled { get; set; }

        public bool RequireSecret { get; set; }
    }
}