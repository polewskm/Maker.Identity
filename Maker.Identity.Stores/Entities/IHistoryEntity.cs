using System;

namespace Maker.Identity.Stores.Entities
{
    public interface IHistoryEntity<TBase>
        where TBase : ISupportAssign<TBase>
    {
        long TransactionId { get; set; }

        DateTime CreatedWhenUtc { get; set; }

        DateTime RetiredWhenUtc { get; set; }
    }
}