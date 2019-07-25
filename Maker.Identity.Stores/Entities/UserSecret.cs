using System;

namespace Maker.Identity.Stores.Entities
{
    public class UserSecretBase : ISupportAssign<UserSecretBase>
    {
        public long UserId { get; set; }

        public long SecretId { get; set; }

        public virtual void Assign(UserSecretBase other)
        {
            UserId = other.UserId;
            SecretId = other.SecretId;
        }
    }

    public class UserSecret : UserSecretBase
    {
        public Secret Secret { get; set; }
    }

    public class UserSecretHistory : UserSecretBase, IHistoryEntity<UserSecretBase>
    {
        /// <inheritdoc/>
        public long TransactionId { get; set; }

        /// <inheritdoc/>
        public DateTime CreatedWhenUtc { get; set; }

        /// <inheritdoc/>
        public DateTime RetiredWhenUtc { get; set; }
    }
}