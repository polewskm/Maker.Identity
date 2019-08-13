using System;

namespace Maker.Identity.Contracts.Entities
{
    public class Role : ISupportConcurrencyToken, ISupportId
    {
        /// <inheritdoc/>
        public string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Gets or sets the primary key for this role.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the normalized name for this role.
        /// </summary>
        public string NormalizedName { get; set; }

        /// <summary>
        /// Gets or sets the name for this role.
        /// </summary>
        public string Name { get; set; }

        /// <inheritdoc/>
        public override string ToString() => Name;
    }
}