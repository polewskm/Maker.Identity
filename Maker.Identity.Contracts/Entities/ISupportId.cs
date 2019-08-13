using System;

namespace Maker.Identity.Contracts.Entities
{
    public interface ISupportId : ISupportId<long>
    {
        // nothing
    }

    public interface ISupportId<out TKey>
        where TKey : IEquatable<TKey>
    {
        TKey Id { get; }
    }
}