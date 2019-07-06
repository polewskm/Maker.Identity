using System;

namespace Maker.Identity.Stores.Entities
{
    public interface IHistoryEntity<TBase>
        where TBase : ISupportAssign<TBase>
    {
        string TransactionId { get; set; }

        DateTimeOffset CreatedWhen { get; set; }

        DateTimeOffset RetiredWhen { get; set; }
    }
}