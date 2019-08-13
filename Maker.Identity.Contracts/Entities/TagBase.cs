using System;

namespace Maker.Identity.Contracts.Entities
{
    public abstract class TagBase : ISupportConcurrencyToken, ISupportId
    {
        /// <inheritdoc/>
        public string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();

        public long Id { get; set; }

        public string NormalizedKey { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }
    }
}