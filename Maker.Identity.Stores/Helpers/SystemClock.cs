using System;

namespace Maker.Identity.Stores.Helpers
{
    /// <summary>
    /// Abstracts the system clock to facilitate testing.
    /// </summary>
    public interface ISystemClock
    {
        /// <summary>
        /// Retrieves the current system time in UTC.
        /// </summary>
        DateTimeOffset UtcNow { get; }
    }

    /// <inheritdoc/>
    public class SystemClock : ISystemClock
    {
        /// <inheritdoc/>
        public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
    }
}