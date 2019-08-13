using System;

namespace Maker.Identity.Contracts.Entities
{
    public class Secret : ISupportConcurrencyToken, ISupportId
    {
        /// <inheritdoc/>
        public string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Gets or sets the primary key for this instance.
        /// </summary>
        public long Id { get; set; }

        public string CipherType { get; set; }

        public string CipherText { get; set; }
    }
}